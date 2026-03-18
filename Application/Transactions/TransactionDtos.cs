namespace Disputes.Api.Application.Transactions
{
    public sealed record TransactionListItemDto(
        Guid Id,
        string AccountId,
        DateTimeOffset PostedAt,
        string Merchant,
        string? Description,
        decimal Amount,
        string Currency,
        string? Reference,
        bool IsDisputed,
        Guid? DisputeId
    );

    public sealed record TransactionDetailDto(
        Guid Id,
        string AccountId,
        DateTimeOffset PostedAt,
        string Merchant,
        string? Description,
        decimal Amount,
        string Currency,
        string? Reference,
        bool IsDisputed
    );

    public sealed record PagedResponse<T>(
        IReadOnlyList<T> Items,
        int Page,
        int PageSize,
        int TotalCount
    );
}