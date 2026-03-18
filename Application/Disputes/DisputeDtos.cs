namespace Disputes.Api.Application.Disputes
{
    public sealed record CreateDisputeRequest(
        Guid TransactionId,
        string ReasonCode,
        string? CustomerComment
    );

    public sealed record CreateDisputeResponse(
        Guid Id,
        Guid TransactionId,
        string Status,
        string ReasonCode,
        string? CustomerComment,
        DateTimeOffset CreatedAt
    );

    public sealed record ChangeDisputeStatusRequest(
        string ToStatus,
        string? Comment
    );

    public sealed record ChangeDisputeStatusResponse(
        Guid Id,
        string Status,
        DateTimeOffset UpdatedAt,
        DateTimeOffset? ClosedAt
    );

    public sealed record DisputeListItemDto(
        Guid Id,
        Guid TransactionId,
        string AccountId,
        string Merchant,
        DateTimeOffset PostedAt,
        decimal Amount,
        string Currency,
        string Status,
        string ReasonCode,
        string? CustomerComment,
        DateTimeOffset CreatedAt,
        DateTimeOffset UpdatedAt
    );

    public sealed record DisputeEventDto(
        Guid Id,
        string EventType,
        string? FromStatus,
        string? ToStatus,
        string? Comment,
        Guid CreatedByUserId,
        string CreatedByUserName,
        DateTimeOffset CreatedAt
    );

    public sealed record DisputeDetailDto(
        Guid Id,
        Guid TransactionId,
        string AccountId,
        string Merchant,
        DateTimeOffset PostedAt,
        decimal Amount,
        string Currency,
        string Status,
        string ReasonCode,
        string? CustomerComment,
        Guid OpenedByUserId,
        string OpenedByUserName,
        DateTimeOffset CreatedAt,
        DateTimeOffset UpdatedAt,
        DateTimeOffset? ClosedAt,
        IReadOnlyList<DisputeEventDto> Events
    );
}