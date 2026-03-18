namespace Disputes.Api.Domain.Entities
{
    public sealed class Transaction
    {
        public Guid Id { get; set; }
        public string AccountId { get; set; } = default!;
        public DateTimeOffset PostedAt { get; set; }
        public string Merchant { get; set; } = default!;
        public string? Description { get; set; }
        public decimal Amount { get; set; }
        public string Currency { get; set; } = "ZAR";
        public string? Reference { get; set; }
        public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;

        public Dispute? Dispute { get; set; }
    }
}
