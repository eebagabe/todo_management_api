using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TodoManagement.Api.Data;
using TodoManagement.Api.Models;
using TodoManagement.Api.Requests;

namespace TodoManagement.Api.Controllers;

[ApiController]
[Route("api")]
public class TaskController(AppDbContext db) : ControllerBase
{
    [HttpGet("tasks")]
    public async Task<IActionResult> Index([FromQuery] TaskFilterRequest filter)
    {
        var query = db.Tasks.Include(t => t.Project).Include(t => t.Labels);
        return Ok(await Filter(query, filter).ToListAsync());
    }

    [HttpGet("projects/{projectId}/tasks")]
    public async Task<IActionResult> IndexForProject(long projectId, [FromQuery] TaskFilterRequest filter)
    {
        if (!await db.Projects.AnyAsync(p => p.Id == projectId))
        {
            return NotFound();
        }

        var query = db.Tasks.Where(t => t.ProjectId == projectId);
        return Ok(await Filter(query, filter).ToListAsync());
    }

    [HttpPost("tasks")]
    public async Task<IActionResult> Store(TaskRequest request)
    {
        if (!await db.Projects.AnyAsync(p => p.Id == request.ProjectId))
        {
            ModelState.AddModelError(nameof(request.ProjectId), "The selected project id is invalid.");
            return ValidationProblem(ModelState);
        }

        var now = DateTime.UtcNow;

        var task = new TaskItem
        {
            Title = request.Title,
            Description = request.Description,
            Status = request.Status,
            Priority = request.Priority,
            EstimatedHours = request.EstimatedHours,
            ActualHours = request.ActualHours,
            StartDate = request.StartDate,
            DueDate = request.DueDate,
            ProjectId = request.ProjectId,
            CompletedAt = request.Status == "completed" ? now : null,
            CreatedAt = now,
            UpdatedAt = now,
        };

        if (request.LabelIds is not null)
        {
            task.Labels = await db.Labels.Where(l => request.LabelIds.Contains(l.Id)).ToListAsync();
        }

        db.Tasks.Add(task);
        await db.SaveChangesAsync();
        await db.Entry(task).Reference(t => t.Project).LoadAsync();

        return StatusCode(StatusCodes.Status201Created, task);
    }

    [HttpGet("tasks/{id}")]
    public async Task<IActionResult> Show(long id)
    {
        var task = await db.Tasks
            .Include(t => t.Project)
            .Include(t => t.Labels)
            .Include(t => t.Comments)
            .FirstOrDefaultAsync(t => t.Id == id);

        return task is null ? NotFound() : Ok(task);
    }

    [HttpPut("tasks/{id}")]
    public async Task<IActionResult> Update(long id, UpdateTaskRequest request)
    {
        var task = await db.Tasks.Include(t => t.Labels).FirstOrDefaultAsync(t => t.Id == id);

        if (task is null)
        {
            return NotFound();
        }

        if (request.ProjectId is not null && !await db.Projects.AnyAsync(p => p.Id == request.ProjectId))
        {
            ModelState.AddModelError(nameof(request.ProjectId), "The selected project id is invalid.");
            return ValidationProblem(ModelState);
        }

        var now = DateTime.UtcNow;

        if (request.Status is not null)
        {
            task.CompletedAt = request.Status == "completed" ? task.CompletedAt ?? now : null;
        }

        task.Title = request.Title ?? task.Title;
        task.Description = request.Description ?? task.Description;
        task.Status = request.Status ?? task.Status;
        task.Priority = request.Priority ?? task.Priority;
        task.EstimatedHours = request.EstimatedHours ?? task.EstimatedHours;
        task.ActualHours = request.ActualHours ?? task.ActualHours;
        task.StartDate = request.StartDate ?? task.StartDate;
        task.DueDate = request.DueDate ?? task.DueDate;
        task.ProjectId = request.ProjectId ?? task.ProjectId;
        task.UpdatedAt = now;

        if (request.LabelIds is not null)
        {
            task.Labels = await db.Labels.Where(l => request.LabelIds.Contains(l.Id)).ToListAsync();
        }

        await db.SaveChangesAsync();
        await db.Entry(task).Reference(t => t.Project).LoadAsync();

        return Ok(task);
    }

    [HttpDelete("tasks/{id}")]
    public async Task<IActionResult> Destroy(long id)
    {
        var deleted = await db.Tasks.Where(t => t.Id == id).ExecuteDeleteAsync();
        return deleted == 0 ? NotFound() : NoContent();
    }

    private static IQueryable<TaskItem> Filter(IQueryable<TaskItem> query, TaskFilterRequest filter)
    {
        if (filter.Status is not null)
        {
            query = query.Where(t => t.Status == filter.Status);
        }

        if (filter.CreatedAt is not null)
        {
            query = query.Where(t => DateOnly.FromDateTime(t.CreatedAt) == filter.CreatedAt);
        }

        var ascending = filter.OrderDirection == "asc";

        return filter.OrderBy switch
        {
            "title" => ascending ? query.OrderBy(t => t.Title) : query.OrderByDescending(t => t.Title),
            "created_at" => ascending ? query.OrderBy(t => t.CreatedAt) : query.OrderByDescending(t => t.CreatedAt),
            _ => query,
        };
    }
}
