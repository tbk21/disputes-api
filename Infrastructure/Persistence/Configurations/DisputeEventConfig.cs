using Disputes.Api.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Disputes.Api.Infrastructure.Persistence.Configurations;

public sealed class DisputeEventConfig : IEntityTypeConfiguration<DisputeEvent>
{
    public void Configure(EntityTypeBuilder<DisputeEvent> b)
    {
        b.ToTable("dispute_events");
        b.HasKey(x => x.Id);

        b.Property(x => x.EventType).HasConversion<string>();
        b.Property(x => x.FromStatus).HasConversion<string?>();
        b.Property(x => x.ToStatus).HasConversion<string?>();

        b.HasIndex(x => new { x.DisputeId, x.CreatedAt });

        b.HasOne(x => x.CreatedByUser)
            .WithMany()
            .HasForeignKey(x => x.CreatedByUserId)
            .OnDelete(DeleteBehavior.NoAction);
    }
}