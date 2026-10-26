using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;

namespace TodoManagement.Api.Requests;

public class TaskRequest
{
    [Required]
    public required string Title { get; init; }

    public string? Description { get; init; }

    [Required, AllowedValues("pending", "in_progress", "completed")]
    public required string Status { get; init; }

    [Required, AllowedValues("low", "medium", "high", "urgent")]
    public required string Priority { get; init; }

    public decimal? EstimatedHours { get; init; }

    public decimal? ActualHours { get; init; }

    public DateOnly? StartDate { get; init; }

    public DateOnly? DueDate { get; init; }

    public required long ProjectId { get; init; }

    public List<long>? LabelIds { get; init; }
}

// shortcut: null means "keep current value", so nullable fields cannot be cleared on update; switch to JSON patch if the frontend needs that.
public class UpdateTaskRequest
{
    public string? Title { get; init; }

    public string? Description { get; init; }

    [AllowedValues(null, "pending", "in_progress", "completed")]
    public string? Status { get; init; }

    [AllowedValues(null, "low", "medium", "high", "urgent")]
    public string? Priority { get; init; }

    public decimal? EstimatedHours { get; init; }

    public decimal? ActualHours { get; init; }

    public DateOnly? StartDate { get; init; }

    public DateOnly? DueDate { get; init; }

    public long? ProjectId { get; init; }

    public List<long>? LabelIds { get; init; }
}

public class TaskFilterRequest
{
    [FromQuery(Name = "status")]
    public string? Status { get; init; }

    [FromQuery(Name = "created_at")]
    public DateOnly? CreatedAt { get; init; }

    [FromQuery(Name = "order_by")]
    public string OrderBy { get; init; } = "created_at";

    [FromQuery(Name = "order_direction")]
    public string OrderDirection { get; init; } = "desc";
}
