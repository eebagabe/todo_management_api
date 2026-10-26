using System.ComponentModel.DataAnnotations;

namespace TodoManagement.Api.Requests;

public class LabelRequest
{
    [Required]
    public required string Name { get; init; }

    [Required]
    public required string Color { get; init; }
}

public class UpdateLabelRequest
{
    public string? Name { get; init; }

    public string? Color { get; init; }
}
