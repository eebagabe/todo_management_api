namespace TodoManagement.Api.Models;

public class Comment
{
    public long Id { get; set; }
    public long TaskId { get; set; }
    public string Content { get; set; } = "";
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }

    public TaskItem? Task { get; set; }
}
