using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TodoManagement.Api.Data;
using TodoManagement.Api.Models;
using TodoManagement.Api.Requests;

namespace TodoManagement.Api.Controllers;

[ApiController]
[Route("api/labels")]
public class LabelController(AppDbContext db) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> Index()
    {
        return Ok(await db.Labels.ToListAsync());
    }

    [HttpPost]
    public async Task<IActionResult> Store(LabelRequest request)
    {
        if (await db.Labels.AnyAsync(l => l.Name == request.Name))
        {
            ModelState.AddModelError(nameof(request.Name), "The name has already been taken.");
            return ValidationProblem(ModelState);
        }

        var now = DateTime.UtcNow;

        var label = new Label
        {
            Name = request.Name,
            Color = request.Color,
            CreatedAt = now,
            UpdatedAt = now,
        };

        db.Labels.Add(label);
        await db.SaveChangesAsync();

        return StatusCode(StatusCodes.Status201Created, label);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> Show(long id)
    {
        var label = await db.Labels.FindAsync(id);
        return label is null ? NotFound() : Ok(label);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(long id, UpdateLabelRequest request)
    {
        var label = await db.Labels.FindAsync(id);

        if (label is null)
        {
            return NotFound();
        }

        if (request.Name is not null && await db.Labels.AnyAsync(l => l.Name == request.Name && l.Id != id))
        {
            ModelState.AddModelError(nameof(request.Name), "The name has already been taken.");
            return ValidationProblem(ModelState);
        }

        label.Name = request.Name ?? label.Name;
        label.Color = request.Color ?? label.Color;
        label.UpdatedAt = DateTime.UtcNow;

        await db.SaveChangesAsync();

        return Ok(label);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Destroy(long id)
    {
        var deleted = await db.Labels.Where(l => l.Id == id).ExecuteDeleteAsync();
        return deleted == 0 ? NotFound() : NoContent();
    }
}
