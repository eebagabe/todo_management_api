using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TodoManagement.Api.Data;
using TodoManagement.Api.Models;
using TodoManagement.Api.Requests;

namespace TodoManagement.Api.Controllers;

[ApiController]
[Route("api/comments")]
public class CommentController(AppDbContext db) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> Index()
    {
        return Ok(await db.Comments.Include(c => c.Task).ToListAsync());
    }

    [HttpPost]
    public async Task<IActionResult> Store(CommentRequest request)
    {
        if (!await db.Tasks.AnyAsync(t => t.Id == request.TaskId))
        {
            ModelState.AddModelError(nameof(request.TaskId), "The selected task id is invalid.");
            return ValidationProblem(ModelState);
        }

        var now = DateTime.UtcNow;

        var comment = new Comment
        {
            TaskId = request.TaskId,
            Content = request.Content,
            CreatedAt = now,
            UpdatedAt = now,
        };

        db.Comments.Add(comment);
        await db.SaveChangesAsync();

        return StatusCode(StatusCodes.Status201Created, comment);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> Show(long id)
    {
        var comment = await db.Comments.Include(c => c.Task).FirstOrDefaultAsync(c => c.Id == id);
        return comment is null ? NotFound() : Ok(comment);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(long id, UpdateCommentRequest request)
    {
        var comment = await db.Comments.FindAsync(id);

        if (comment is null)
        {
            return NotFound();
        }

        comment.Content = request.Content;
        comment.UpdatedAt = DateTime.UtcNow;

        await db.SaveChangesAsync();

        return Ok(comment);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Destroy(long id)
    {
        var deleted = await db.Comments.Where(c => c.Id == id).ExecuteDeleteAsync();
        return deleted == 0 ? NotFound() : NoContent();
    }
}
