using Disputes.Api.Domain.Enums;

namespace Disputes.Api.Domain.Entities
{
    public sealed class DisputeEvent
    {
        public Guid Id { get; set; }

        public Guid DisputeId { get; set; }
        public Dispute Dispute { get; set; } = default!;

        public DisputeEventType EventType { get; set; }

        public DisputeStatus? FromStatus { get; set; }
        public DisputeStatus? ToStatus { get; set; }

        public string? Comment { get; set; }

        public Guid CreatedByUserId { get; set; }
        public User CreatedByUser { get; set; } = default!;

        public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;
    }
}
