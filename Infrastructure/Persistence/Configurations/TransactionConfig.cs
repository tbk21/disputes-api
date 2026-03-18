using Disputes.Api.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Disputes.Api.Infrastructure.Persistence.Configurations
{
    public sealed class TransactionConfig : IEntityTypeConfiguration<Transaction>
    {
        public void Configure(EntityTypeBuilder<Transaction> b)
        {
            b.ToTable("transactions");
            b.HasKey(x => x.Id);

            b.Property(x => x.Amount).HasColumnType("decimal(18,2)");
            b.Property(x => x.Currency).HasMaxLength(3);

            b.HasIndex(x => new { x.AccountId, x.PostedAt });
            b.HasIndex(x => x.Merchant);

            b.HasOne(x => x.Dispute)
                .WithOne(x => x.Transaction)
                .HasForeignKey<Dispute>(x => x.TransactionId);
        }
    }
}
