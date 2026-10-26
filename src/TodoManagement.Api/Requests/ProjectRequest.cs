using System.ComponentModel.DataAnnotations;

namespace TodoManagement.Api.Requests;

public class ProjectRequest
{
    [Required]
    public required string Name { get; init; }

    public string? Description { get; init; }

    public required DateOnly StartDate { get; init; }

    public DateOnly? EndDate { get; init; }

    [Required, AllowedValues("planning", "active", "on_hold", "completed")]
    public required string Status { get; init; }
}

// shortcut: null means "keep current value", so nullable fields cannot be cleared on update; switch to JSON patch if the frontend needs that.
public class UpdateProjectRequest
{
    public string? Name { get; init; }

    public string? Description { get; init; }

    public DateOnly? StartDate { get; init; }

    public DateOnly? EndDate { get; init; }

    [AllowedValues(null, "planning", "active", "on_hold", "completed")]
    public string? Status { get; init; }
}
