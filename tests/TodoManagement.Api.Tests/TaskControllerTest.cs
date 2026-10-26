using System.ComponentModel.DataAnnotations;
using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TodoManagement.Api.Controllers;
using TodoManagement.Api.Data;
using TodoManagement.Api.Models;
using TodoManagement.Api.Requests;

namespace TodoManagement.Api.Tests;

public class TaskControllerTest
{
    private readonly AppDbContext _db;
    private readonly TaskController _controller;

    public TaskControllerTest()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        _db = new AppDbContext(options);
        _db.Projects.Add(new Project { Id = 1, Name = "Projeto", Status = "active", StartDate = new DateOnly(2026, 1, 1) });
        _db.SaveChanges();

        _controller = new TaskController(_db);
    }

    private static TaskRequest Request(string title, string status) => new()
    {
        Title = title,
        Status = status,
        Priority = "low",
        ProjectId = 1,
    };

    private static T Value<T>(IActionResult result) => (T)((ObjectResult)result).Value!;

    [Fact]
    public async Task StoreCompletedTaskSetsCompletedAt()
    {
        var task = Value<TaskItem>(await _controller.Store(Request("Deploy", "completed")));

        Assert.NotNull(task.CompletedAt);
        Assert.NotNull(task.Project);
    }

    [Fact]
    public async Task UpdateKeepsCompletedAtUnlessStatusChanges()
    {
        var task = Value<TaskItem>(await _controller.Store(Request("Deploy", "completed")));
        var completedAt = task.CompletedAt;

        await _controller.Update(task.Id, new UpdateTaskRequest { Title = "Deploy v2" });
        Assert.Equal(completedAt, task.CompletedAt);

        await _controller.Update(task.Id, new UpdateTaskRequest { Status = "completed" });
        Assert.Equal(completedAt, task.CompletedAt);

        await _controller.Update(task.Id, new UpdateTaskRequest { Status = "pending" });
        Assert.Null(task.CompletedAt);
    }

    [Fact]
    public async Task IndexFiltersByStatusAndOrdersByTitle()
    {
        await _controller.Store(Request("B", "pending"));
        await _controller.Store(Request("A", "pending"));
        await _controller.Store(Request("C", "completed"));

        var tasks = Value<List<TaskItem>>(await _controller.Index(new TaskFilterRequest
        {
            Status = "pending",
            OrderBy = "title",
            OrderDirection = "asc",
        }));

        Assert.Equal(["A", "B"], tasks.Select(t => t.Title));
    }

    [Fact]
    public async Task ShowWithCommentsSerializesWithoutCycles()
    {
        var task = Value<TaskItem>(await _controller.Store(Request("Deploy", "pending")));
        _db.Comments.Add(new Comment { TaskId = task.Id, Content = "ok" });
        await _db.SaveChangesAsync();

        var shown = Value<TaskItem>(await _controller.Show(task.Id));

        // Same options as Program.cs.
        var json = JsonSerializer.Serialize(shown, new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower,
            ReferenceHandler = ReferenceHandler.IgnoreCycles,
        });

        Assert.Contains("\"content\":\"ok\"", json);
    }

    [Fact]
    public void UpdateRequestAllowsMissingStatusButRejectsUnknownStatus()
    {
        bool IsValid(object request) => Validator.TryValidateObject(request, new ValidationContext(request), null, true);

        Assert.True(IsValid(new UpdateTaskRequest()));
        Assert.False(IsValid(new UpdateTaskRequest { Status = "done" }));
    }
}
