using Disputes.Api.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Reflection.Emit;

namespace Disputes.Api.Infrastructure.Persistence
{
    public sealed class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<User> Users => Set<User>();
        public DbSet<Transaction> Transactions => Set<Transaction>();
        public DbSet<Dispute> Disputes => Set<Dispute>();
        public DbSet<DisputeEvent> DisputeEvents => Set<DisputeEvent>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
            => modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);
    }
}
