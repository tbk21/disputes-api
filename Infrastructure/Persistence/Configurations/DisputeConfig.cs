using Disputes.Api.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Disputes.Api.Infrastructure.Persistence.Configurations;

public sealed class DisputeConfig : IEntityTypeConfiguration<Dispute>
{
    public void Configure(EntityTypeBuilder<Dispute> b)
    {
        b.ToTable("disputes");
        b.HasKey(x => x.Id);

        b.Property(x => x.Status).HasConversion<string>();
        b.Property(x => x.ReasonCode).HasMaxLength(50).IsRequired();

        b.HasIndex(x => x.TransactionId).IsUnique();

        b.HasOne(x => x.Transaction)
            .WithOne(x => x.Dispute)
            .HasForeignKey<Dispute>(x => x.TransactionId)
            .OnDelete(DeleteBehavior.Cascade);

        b.HasOne(x => x.OpenedByUser)
            .WithMany()
            .HasForeignKey(x => x.OpenedByUserId)
            .OnDelete(DeleteBehavior.NoAction);

        b.HasMany(x => x.Events)
            .WithOne(e => e.Dispute)
            .HasForeignKey(e => e.DisputeId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}