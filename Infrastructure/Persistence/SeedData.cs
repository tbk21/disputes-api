using BCrypt.Net;
using Disputes.Api.Domain.Entities;
using Disputes.Api.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace Disputes.Api.Infrastructure.Persistence;

public static class SeedData
{
    public static async Task EnsureSeededAsync(AppDbContext db)
    {
        await db.Database.EnsureCreatedAsync();

        var now = DateTimeOffset.UtcNow;

        // Seed users
        var customer1 = await db.Users.FirstOrDefaultAsync(x => x.Email == "customer1@demo.local");
        if (customer1 is null)
        {
            customer1 = new User
            {
                Id = Guid.NewGuid(),
                Email = "customer1@demo.local",
                DisplayName = "Customer One",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("Password123!"),
                Role = "Customer",
                CreatedAt = now
            };

            db.Users.Add(customer1);
        }

        var customer2 = await db.Users.FirstOrDefaultAsync(x => x.Email == "customer2@demo.local");
        if (customer2 is null)
        {
            customer2 = new User
            {
                Id = Guid.NewGuid(),
                Email = "customer2@demo.local",
                DisplayName = "Customer Two",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("Password123!"),
                Role = "Customer",
                CreatedAt = now
            };

            db.Users.Add(customer2);
        }

        var customer3 = await db.Users.FirstOrDefaultAsync(x => x.Email == "customer3@demo.local");
        if (customer3 is null)
        {
            customer3 = new User
            {
                Id = Guid.NewGuid(),
                Email = "customer3@demo.local",
                DisplayName = "Customer Three",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("Password123!"),
                Role = "Customer",
                CreatedAt = now
            };

            db.Users.Add(customer3);
        }

        var agent1 = await db.Users.FirstOrDefaultAsync(x => x.Email == "agent1@demo.local");
        if (agent1 is null)
        {
            agent1 = new User
            {
                Id = Guid.NewGuid(),
                Email = "agent1@demo.local",
                DisplayName = "Support Agent",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("Password123!"),
                Role = "Agent",
                CreatedAt = now
            };

            db.Users.Add(agent1);
        }

        var admin = await db.Users.FirstOrDefaultAsync(x => x.Email == "admin@demo.local");
        if (admin is null)
        {
            admin = new User
            {
                Id = Guid.NewGuid(),
                Email = "admin@demo.local",
                DisplayName = "System Admin",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("Password123!"),
                Role = "Admin",
                CreatedAt = now
            };

            db.Users.Add(admin);
        }

        await db.SaveChangesAsync();

        // Seed transactions only if none exist yet
        var hasTransactions = await db.Transactions.AnyAsync();
        if (!hasTransactions)
        {
            var transactions = new List<Transaction>
        {
            new Transaction
            {
                Id = Guid.NewGuid(),
                AccountId = "ACC-1001",
                PostedAt = now.AddDays(-1).AddHours(-2),
                Merchant = "Game",
                Description = "Card purchase",
                Amount = -543.24m,
                Currency = "ZAR",
                Reference = "REF-10001",
                CreatedAt = now
            },
            new Transaction
            {
                Id = Guid.NewGuid(),
                AccountId = "ACC-1001",
                PostedAt = now.AddDays(-1).AddHours(-5),
                Merchant = "Takealot",
                Description = "Online purchase",
                Amount = -324.52m,
                Currency = "ZAR",
                Reference = "REF-10002",
                CreatedAt = now
            },
            new Transaction
            {
                Id = Guid.NewGuid(),
                AccountId = "ACC-1002",
                PostedAt = now.AddDays(-2).AddHours(-3),
                Merchant = "Dis-Chem",
                Description = "Pharmacy purchase",
                Amount = -908.76m,
                Currency = "ZAR",
                Reference = "REF-10003",
                CreatedAt = now
            },
            new Transaction
            {
                Id = Guid.NewGuid(),
                AccountId = "ACC-1002",
                PostedAt = now.AddDays(-2).AddHours(-10),
                Merchant = "Woolworths",
                Description = "Groceries",
                Amount = -80.70m,
                Currency = "ZAR",
                Reference = "REF-10004",
                CreatedAt = now
            },
            new Transaction
            {
                Id = Guid.NewGuid(),
                AccountId = "ACC-1003",
                PostedAt = now.AddDays(-3).AddHours(-2),
                Merchant = "Spur",
                Description = "Restaurant bill",
                Amount = -2349.62m,
                Currency = "ZAR",
                Reference = "REF-10005",
                CreatedAt = now
            },
            new Transaction
            {
                Id = Guid.NewGuid(),
                AccountId = "ACC-1003",
                PostedAt = now.AddDays(-3).AddHours(-6),
                Merchant = "Engen",
                Description = "Fuel",
                Amount = -1654.69m,
                Currency = "ZAR",
                Reference = "REF-10006",
                CreatedAt = now
            },
            new Transaction
            {
                Id = Guid.NewGuid(),
                AccountId = "ACC-1001",
                PostedAt = now.AddDays(-4).AddHours(-1),
                Merchant = "Uber",
                Description = "Trip fare",
                Amount = -2051.27m,
                Currency = "ZAR",
                Reference = "REF-10007",
                CreatedAt = now
            },
            new Transaction
            {
                Id = Guid.NewGuid(),
                AccountId = "ACC-1002",
                PostedAt = now.AddDays(-4).AddHours(-4),
                Merchant = "Clicks",
                Description = "Store purchase",
                Amount = -121.17m,
                Currency = "ZAR",
                Reference = "REF-10008",
                CreatedAt = now
            },
            new Transaction
            {
                Id = Guid.NewGuid(),
                AccountId = "ACC-1003",
                PostedAt = now.AddDays(-5).AddHours(-2),
                Merchant = "Pick n Pay",
                Description = "Groceries",
                Amount = -1590.16m,
                Currency = "ZAR",
                Reference = "REF-10009",
                CreatedAt = now
            },
            new Transaction
            {
                Id = Guid.NewGuid(),
                AccountId = "ACC-1001",
                PostedAt = now.AddDays(-5).AddHours(-7),
                Merchant = "Bolt",
                Description = "Ride fare",
                Amount = -445.64m,
                Currency = "ZAR",
                Reference = "REF-10010",
                CreatedAt = now
            },
            new Transaction
            {
                Id = Guid.NewGuid(),
                AccountId = "ACC-1002",
                PostedAt = now.AddDays(-6).AddHours(-2),
                Merchant = "Mr D",
                Description = "Food delivery",
                Amount = -550.08m,
                Currency = "ZAR",
                Reference = "REF-10011",
                CreatedAt = now
            },
            new Transaction
            {
                Id = Guid.NewGuid(),
                AccountId = "ACC-1003",
                PostedAt = now.AddDays(-6).AddHours(-9),
                Merchant = "Checkers",
                Description = "Groceries",
                Amount = -275.30m,
                Currency = "ZAR",
                Reference = "REF-10012",
                CreatedAt = now
            }
        };

            db.Transactions.AddRange(transactions);
            await db.SaveChangesAsync();
        }

        // Seed a few demo disputes only if none exist yet
        var hasDisputes = await db.Disputes.AnyAsync();
        if (!hasDisputes)
        {
            var gameTransaction = await db.Transactions.FirstAsync(x => x.Merchant == "Game");
            var dischemTransaction = await db.Transactions.FirstAsync(x => x.Merchant == "Dis-Chem");
            var woolworthsTransaction = await db.Transactions.FirstAsync(x => x.Merchant == "Woolworths");
            var mrdTransaction = await db.Transactions.FirstAsync(x => x.Merchant == "Mr D");

            var dispute1Now = now.AddMinutes(-40);
            var dispute1 = new Dispute
            {
                Id = Guid.NewGuid(),
                TransactionId = gameTransaction.Id,
                AccountId = gameTransaction.AccountId,
                Status = DisputeStatus.Opened,
                ReasonCode = "FRAUD",
                CustomerComment = "I did not authorize this transaction.",
                OpenedByUserId = customer1!.Id,
                CreatedAt = dispute1Now,
                UpdatedAt = dispute1Now
            };
            dispute1.Events.Add(new DisputeEvent
            {
                Id = Guid.NewGuid(),
                DisputeId = dispute1.Id,
                EventType = DisputeEventType.Opened,
                ToStatus = DisputeStatus.Opened,
                Comment = "Dispute created.",
                CreatedByUserId = customer1.Id,
                CreatedAt = dispute1Now
            });

            var dispute2OpenedAt = now.AddMinutes(-90);
            var dispute2ReviewedAt = now.AddMinutes(-60);
            var dispute2 = new Dispute
            {
                Id = Guid.NewGuid(),
                TransactionId = dischemTransaction.Id,
                AccountId = dischemTransaction.AccountId,
                Status = DisputeStatus.InReview,
                ReasonCode = "CARD_NOT_PRESENT",
                CustomerComment = "This looks suspicious and was not done by me.",
                OpenedByUserId = customer2!.Id,
                CreatedAt = dispute2OpenedAt,
                UpdatedAt = dispute2ReviewedAt
            };
            dispute2.Events.Add(new DisputeEvent
            {
                Id = Guid.NewGuid(),
                DisputeId = dispute2.Id,
                EventType = DisputeEventType.Opened,
                ToStatus = DisputeStatus.Opened,
                Comment = "Dispute created.",
                CreatedByUserId = customer2.Id,
                CreatedAt = dispute2OpenedAt
            });
            dispute2.Events.Add(new DisputeEvent
            {
                Id = Guid.NewGuid(),
                DisputeId = dispute2.Id,
                EventType = DisputeEventType.StatusChanged,
                FromStatus = DisputeStatus.Opened,
                ToStatus = DisputeStatus.InReview,
                Comment = "Moved to review by support agent.",
                CreatedByUserId = agent1!.Id,
                CreatedAt = dispute2ReviewedAt
            });

            var dispute3OpenedAt = now.AddHours(-5);
            var dispute3ResolvedAt = now.AddHours(-3);
            var dispute3 = new Dispute
            {
                Id = Guid.NewGuid(),
                TransactionId = woolworthsTransaction.Id,
                AccountId = woolworthsTransaction.AccountId,
                Status = DisputeStatus.Resolved,
                ReasonCode = "DUPLICATE",
                CustomerComment = "I was charged twice for the same purchase.",
                OpenedByUserId = customer3!.Id,
                CreatedAt = dispute3OpenedAt,
                UpdatedAt = dispute3ResolvedAt,
                ClosedAt = dispute3ResolvedAt
            };
            dispute3.Events.Add(new DisputeEvent
            {
                Id = Guid.NewGuid(),
                DisputeId = dispute3.Id,
                EventType = DisputeEventType.Opened,
                ToStatus = DisputeStatus.Opened,
                Comment = "Dispute created.",
                CreatedByUserId = customer3.Id,
                CreatedAt = dispute3OpenedAt
            });
            dispute3.Events.Add(new DisputeEvent
            {
                Id = Guid.NewGuid(),
                DisputeId = dispute3.Id,
                EventType = DisputeEventType.StatusChanged,
                FromStatus = DisputeStatus.Opened,
                ToStatus = DisputeStatus.Resolved,
                Comment = "Duplicate payment confirmed and resolved.",
                CreatedByUserId = admin!.Id,
                CreatedAt = dispute3ResolvedAt
            });

            var dispute4OpenedAt = now.AddDays(-1);
            var dispute4RejectedAt = now.AddHours(-12);
            var dispute4 = new Dispute
            {
                Id = Guid.NewGuid(),
                TransactionId = mrdTransaction.Id,
                AccountId = mrdTransaction.AccountId,
                Status = DisputeStatus.Rejected,
                ReasonCode = "SERVICE",
                CustomerComment = "I want to dispute this delivery charge.",
                OpenedByUserId = customer1.Id,
                CreatedAt = dispute4OpenedAt,
                UpdatedAt = dispute4RejectedAt,
                ClosedAt = dispute4RejectedAt
            };
            dispute4.Events.Add(new DisputeEvent
            {
                Id = Guid.NewGuid(),
                DisputeId = dispute4.Id,
                EventType = DisputeEventType.Opened,
                ToStatus = DisputeStatus.Opened,
                Comment = "Dispute created.",
                CreatedByUserId = customer1.Id,
                CreatedAt = dispute4OpenedAt
            });
            dispute4.Events.Add(new DisputeEvent
            {
                Id = Guid.NewGuid(),
                DisputeId = dispute4.Id,
                EventType = DisputeEventType.StatusChanged,
                FromStatus = DisputeStatus.Opened,
                ToStatus = DisputeStatus.Rejected,
                Comment = "Rejected after review.",
                CreatedByUserId = agent1.Id,
                CreatedAt = dispute4RejectedAt
            });

            db.Disputes.AddRange(dispute1, dispute2, dispute3, dispute4);
            await db.SaveChangesAsync();
        }
    }
}