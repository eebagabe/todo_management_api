using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TodoManagement.Api.Data;
using TodoManagement.Api.Models;
using TodoManagement.Api.Requests;

namespace TodoManagement.Api.Controllers;

[ApiController]
[Route("api")]
public class ProjectController(AppDbContext db) : ControllerBase
{
    [HttpGet("projects")]
    public async Task<IActionResult> Index()
    {
        return Ok(await db.Projects.ToListAsync());
    }

    [HttpPost("projects")]
    public async Task<IActionResult> Store(ProjectRequest request)
    {
        var now = DateTime.UtcNow;

        var project = new Project
        {
            Name = request.Name,
            Description = request.Description,
            StartDate = request.StartDate,
            EndDate = request.EndDate,
            Status = request.Status,
            CreatedAt = now,
            UpdatedAt = now,
        };

        db.Projects.Add(project);
        await db.SaveChangesAsync();

        return StatusCode(StatusCodes.Status201Created, project);
    }

    [HttpGet("projects/{id}")]
    public async Task<IActionResult> Show(long id)
    {
        var project = await db.Projects.FindAsync(id);
        return project is null ? NotFound() : Ok(project);
    }

    [HttpPut("projects/{id}")]
    public async Task<IActionResult> Update(long id, UpdateProjectRequest request)
    {
        var project = await db.Projects.FindAsync(id);

        if (project is null)
        {
            return NotFound();
        }

        project.Name = request.Name ?? project.Name;
        project.Description = request.Description ?? project.Description;
        project.StartDate = request.StartDate ?? project.StartDate;
        project.EndDate = request.EndDate ?? project.EndDate;
        project.Status = request.Status ?? project.Status;
        project.UpdatedAt = DateTime.UtcNow;

        await db.SaveChangesAsync();

        return Ok(project);
    }

    [HttpDelete("projects/{id}")]
    public async Task<IActionResult> Destroy(long id)
    {
        var deleted = await db.Projects.Where(p => p.Id == id).ExecuteDeleteAsync();
        return deleted == 0 ? NotFound() : NoContent();
    }

    [HttpGet("overview")]
    public async Task<IActionResult> Overview()
    {
        return Ok(new
        {
            ProjectsInProgress = await db.Projects.CountAsync(p => p.Status == "active"),
            TotalTasks = await db.Tasks.CountAsync(),
            CompletedTasks = await db.Tasks.CountAsync(t => t.Status == "completed"),
        });
    }
}
