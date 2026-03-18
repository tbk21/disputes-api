using System.Security.Claims;
using Disputes.Api.Application.Disputes;
using Disputes.Api.Domain.Entities;
using Disputes.Api.Domain.Enums;
using Disputes.Api.Infrastructure.Persistence;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Disputes.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public sealed class DisputesController : ControllerBase
    {
        private readonly AppDbContext _db;

        public DisputesController(AppDbContext db)
        {
            _db = db;
        }

        [HttpPost]
        public async Task<ActionResult<CreateDisputeResponse>> CreateDispute(
            [FromBody] CreateDisputeRequest request,
            CancellationToken cancellationToken = default)
        {
            if (request.TransactionId == Guid.Empty)
            {
                return BadRequest(new { message = "TransactionId is required." });
            }

            if (string.IsNullOrWhiteSpace(request.ReasonCode))
            {
                return BadRequest(new { message = "ReasonCode is required." });
            }

            var transaction = await _db.Transactions
                .FirstOrDefaultAsync(x => x.Id == request.TransactionId, cancellationToken);

            if (transaction is null)
            {
                return NotFound(new { message = "Transaction not found." });
            }

            var existingDispute = await _db.Disputes
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.TransactionId == request.TransactionId, cancellationToken);

            if (existingDispute is not null)
            {
                return Conflict(new { message = "A dispute already exists for this transaction." });
            }

            var currentUserIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!Guid.TryParse(currentUserIdClaim, out var currentUserId))
            {
                return Unauthorized(new { message = "Invalid user context." });
            }

            var currentUser = await _db.Users
                .FirstOrDefaultAsync(x => x.Id == currentUserId, cancellationToken);

            if (currentUser is null)
            {
                return Unauthorized(new { message = "User not found." });
            }

            var now = DateTimeOffset.UtcNow;

            var dispute = new Dispute
            {
                Id = Guid.NewGuid(),
                TransactionId = transaction.Id,
                AccountId = transaction.AccountId,
                Status = DisputeStatus.Opened,
                ReasonCode = request.ReasonCode.Trim(),
                CustomerComment = request.CustomerComment?.Trim(),
                OpenedByUserId = currentUser.Id,
                CreatedAt = now,
                UpdatedAt = now
            };

            var disputeEvent = new DisputeEvent
            {
                Id = Guid.NewGuid(),
                DisputeId = dispute.Id,
                EventType = DisputeEventType.Opened,
                FromStatus = null,
                ToStatus = DisputeStatus.Opened,
                Comment = "Dispute created.",
                CreatedByUserId = currentUser.Id,
                CreatedAt = now
            };

            dispute.Events.Add(disputeEvent);

            _db.Disputes.Add(dispute);
            await _db.SaveChangesAsync(cancellationToken);

            var response = new CreateDisputeResponse(
                dispute.Id,
                dispute.TransactionId,
                dispute.Status.ToString(),
                dispute.ReasonCode,
                dispute.CustomerComment,
                dispute.CreatedAt
            );

            return CreatedAtAction(nameof(GetDisputeById), new { id = dispute.Id }, response);
        }

        [HttpGet]
        public async Task<ActionResult<object>> GetDisputes(
            [FromQuery] string? status,
            [FromQuery] string? merchant,
            [FromQuery] DateTimeOffset? from,
            [FromQuery] DateTimeOffset? to,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 20,
            CancellationToken cancellationToken = default)
        {
            page = page < 1 ? 1 : page;
            pageSize = pageSize < 1 ? 20 : pageSize;
            pageSize = pageSize > 200 ? 200 : pageSize;

            var query = _db.Disputes
                .AsNoTracking()
                .Include(x => x.Transaction)
                .Include(x => x.OpenedByUser)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(status))
            {
                if (!Enum.TryParse<DisputeStatus>(status, true, out var parsedStatus))
                {
                    return BadRequest(new { message = "Invalid status value." });
                }

                query = query.Where(x => x.Status == parsedStatus);
            }

            if (!string.IsNullOrWhiteSpace(merchant))
            {
                query = query.Where(x => x.Transaction.Merchant.Contains(merchant));
            }

            if (from.HasValue)
            {
                query = query.Where(x => x.CreatedAt >= from.Value);
            }

            if (to.HasValue)
            {
                query = query.Where(x => x.CreatedAt <= to.Value);
            }

            query = query.OrderByDescending(x => x.CreatedAt);

            var totalCount = await query.CountAsync(cancellationToken);

            var items = await query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(x => new DisputeListItemDto(
                    x.Id,
                    x.TransactionId,
                    x.AccountId,
                    x.Transaction.Merchant,
                    x.Transaction.PostedAt,
                    x.Transaction.Amount,
                    x.Transaction.Currency,
                    x.Status.ToString(),
                    x.ReasonCode,
                    x.CustomerComment,
                    x.CreatedAt,
                    x.UpdatedAt
                ))
                .ToListAsync(cancellationToken);

            return Ok(new
            {
                items,
                page,
                pageSize,
                totalCount
            });
        }

        [HttpGet("{id:guid}")]
        public async Task<ActionResult<DisputeDetailDto>> GetDisputeById(
            Guid id,
            CancellationToken cancellationToken = default)
        {
            var dispute = await _db.Disputes
                .AsNoTracking()
                .Include(x => x.Transaction)
                .Include(x => x.OpenedByUser)
                .Include(x => x.Events)
                    .ThenInclude(x => x.CreatedByUser)
                .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);

            if (dispute is null)
            {
                return NotFound(new { message = "Dispute not found." });
            }

            var response = new DisputeDetailDto(
                dispute.Id,
                dispute.TransactionId,
                dispute.AccountId,
                dispute.Transaction.Merchant,
                dispute.Transaction.PostedAt,
                dispute.Transaction.Amount,
                dispute.Transaction.Currency,
                dispute.Status.ToString(),
                dispute.ReasonCode,
                dispute.CustomerComment,
                dispute.OpenedByUserId,
                dispute.OpenedByUser.DisplayName,
                dispute.CreatedAt,
                dispute.UpdatedAt,
                dispute.ClosedAt,
                dispute.Events
                    .OrderBy(x => x.CreatedAt)
                    .Select(x => new DisputeEventDto(
                        x.Id,
                        x.EventType.ToString(),
                        x.FromStatus?.ToString(),
                        x.ToStatus?.ToString(),
                        x.Comment,
                        x.CreatedByUserId,
                        x.CreatedByUser.DisplayName,
                        x.CreatedAt
                    ))
                    .ToList()
            );

            return Ok(response);
        }



        [HttpPost("{id:guid}/status")]
        public async Task<ActionResult<ChangeDisputeStatusResponse>> ChangeDisputeStatus(
          Guid id,
          [FromBody] ChangeDisputeStatusRequest request,
          CancellationToken cancellationToken = default)
        {
            if (request == null)
            {
                return BadRequest(new { message = "Request body is required." });
            }

            if (string.IsNullOrWhiteSpace(request.ToStatus))
            {
                return BadRequest(new { message = "ToStatus is required." });
            }

            if (!Enum.TryParse<DisputeStatus>(request.ToStatus, true, out var newStatus))
            {
                return BadRequest(new { message = "Invalid status value." });
            }

            var dispute = await _db.Disputes
                .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);

            if (dispute is null)
            {
                return NotFound(new { message = "Dispute not found." });
            }

            if (!IsValidTransition(dispute.Status, newStatus))
            {
                return Conflict(new
                {
                    message = $"Invalid status transition from {dispute.Status} to {newStatus}."
                });
            }

            var currentUserIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!Guid.TryParse(currentUserIdClaim, out var currentUserId))
            {
                return Unauthorized(new { message = "Invalid user context." });
            }

            var currentUserExists = await _db.Users
                .AnyAsync(x => x.Id == currentUserId, cancellationToken);

            if (!currentUserExists)
            {
                return Unauthorized(new { message = "Authenticated user does not exist in the database." });
            }

            var now = DateTimeOffset.UtcNow;
            var previousStatus = dispute.Status;

            dispute.Status = newStatus;
            dispute.UpdatedAt = now;

            if (newStatus == DisputeStatus.Resolved ||
                newStatus == DisputeStatus.Rejected ||
                newStatus == DisputeStatus.Cancelled)
            {
                dispute.ClosedAt = now;
            }
            else
            {
                dispute.ClosedAt = null;
            }

            var disputeEvent = new DisputeEvent
            {
                Id = Guid.NewGuid(),
                DisputeId = dispute.Id,
                EventType = DisputeEventType.StatusChanged,
                FromStatus = previousStatus,
                ToStatus = newStatus,
                Comment = string.IsNullOrWhiteSpace(request.Comment) ? null : request.Comment.Trim(),
                CreatedByUserId = currentUserId,
                CreatedAt = now
            };

            _db.DisputeEvents.Add(disputeEvent);

            await _db.SaveChangesAsync(cancellationToken);

            var response = new ChangeDisputeStatusResponse(
                dispute.Id,
                dispute.Status.ToString(),
                dispute.UpdatedAt,
                dispute.ClosedAt
            );

            return Ok(response);
        }

        private static bool IsValidTransition(DisputeStatus currentStatus, DisputeStatus newStatus)
        {
            return (currentStatus, newStatus) switch
            {
                (DisputeStatus.Opened, DisputeStatus.InReview) => true,
                (DisputeStatus.Opened, DisputeStatus.Cancelled) => true,
                (DisputeStatus.InReview, DisputeStatus.Resolved) => true,
                (DisputeStatus.InReview, DisputeStatus.Rejected) => true,
                _ => false
            };
        }
    }
}