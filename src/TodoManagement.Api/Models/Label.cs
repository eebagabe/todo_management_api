namespace TodoManagement.Api.Models;

public class Label
{
    public long Id { get; set; }
    public string Name { get; set; } = "";
    public string Color { get; set; } = "";
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}
