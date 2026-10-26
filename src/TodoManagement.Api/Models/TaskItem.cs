namespace TodoManagement.Api.Models;

public class TaskItem
{
    public long Id { get; set; }
    public string Title { get; set; } = "";
    public string? Description { get; set; }
    public string Status { get; set; } = "";
    public string Priority { get; set; } = "";
    public decimal? EstimatedHours { get; set; }
    public decimal? ActualHours { get; set; }
    public DateOnly? StartDate { get; set; }
    public DateOnly? DueDate { get; set; }
    public DateTime? CompletedAt { get; set; }
    public long ProjectId { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }

    public Project? Project { get; set; }
    public List<Label> Labels { get; set; } = [];
    public List<Comment> Comments { get; set; } = [];
}
