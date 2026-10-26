using System.ComponentModel.DataAnnotations;

namespace TodoManagement.Api.Requests;

public class CommentRequest
{
    public required long TaskId { get; init; }

    [Required]
    public required string Content { get; init; }
}

public class UpdateCommentRequest
{
    [Required]
    public required string Content { get; init; }
}
