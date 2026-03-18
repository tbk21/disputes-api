using Disputes.Api.Domain.Enums;

namespace Disputes.Api.Domain.Entities
{
    public sealed class Dispute
    {
        public Guid Id { get; set; }

        public Guid TransactionId { get; set; }
        public Transaction Transaction { get; set; } = default!;

        public string AccountId { get; set; } = default!;
        public DisputeStatus Status { get; set; } = DisputeStatus.Opened;

        public string ReasonCode { get; set; } = default!;
        public string? CustomerComment { get; set; }

        public Guid OpenedByUserId { get; set; }
        public User OpenedByUser { get; set; } = default!;

        public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;
        public DateTimeOffset UpdatedAt { get; set; } = DateTimeOffset.UtcNow;
        public DateTimeOffset? ClosedAt { get; set; }

        public List<DisputeEvent> Events { get; set; } = new();
    }
}
