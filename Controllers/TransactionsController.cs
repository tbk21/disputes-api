using Disputes.Api.Application.Transactions;
using Disputes.Api.Infrastructure.Persistence;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Disputes.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public sealed class TransactionsController : ControllerBase
    {
        private readonly AppDbContext _db;

        public TransactionsController(AppDbContext db)
        {
            _db = db;
        }

        [HttpGet]
        public async Task<ActionResult<PagedResponse<TransactionListItemDto>>> GetTransactions(
            [FromQuery] string? merchant,
            [FromQuery] DateTimeOffset? from,
            [FromQuery] DateTimeOffset? to,
            [FromQuery] decimal? minAmount,
            [FromQuery] decimal? maxAmount,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 20,
            [FromQuery] string? sort = "postedAt_desc",
            CancellationToken cancellationToken = default)
        {
            page = page < 1 ? 1 : page;
            pageSize = pageSize < 1 ? 20 : pageSize;
            pageSize = pageSize > 200 ? 200 : pageSize;

            var query = _db.Transactions
                .AsNoTracking()
                .Include(x => x.Dispute)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(merchant))
            {
                query = query.Where(x => x.Merchant.Contains(merchant));
            }

            if (from.HasValue)
            {
                query = query.Where(x => x.PostedAt >= from.Value);
            }

            if (to.HasValue)
            {
                query = query.Where(x => x.PostedAt <= to.Value);
            }

            if (minAmount.HasValue)
            {
                query = query.Where(x => x.Amount >= minAmount.Value);
            }

            if (maxAmount.HasValue)
            {
                query = query.Where(x => x.Amount <= maxAmount.Value);
            }

            query = sort?.ToLowerInvariant() switch
            {
                "amount_asc" => query.OrderBy(x => x.Amount),
                "amount_desc" => query.OrderByDescending(x => x.Amount),
                "merchant_asc" => query.OrderBy(x => x.Merchant),
                "merchant_desc" => query.OrderByDescending(x => x.Merchant),
                "postedat_asc" => query.OrderBy(x => x.PostedAt),
                _ => query.OrderByDescending(x => x.PostedAt)
            };

            var totalCount = await query.CountAsync(cancellationToken);

            var items = await query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(x => new TransactionListItemDto(
                    x.Id,
                    x.AccountId,
                    x.PostedAt,
                    x.Merchant,
                    x.Description,
                    x.Amount,
                    x.Currency,
                    x.Reference,
                    x.Dispute != null,
                    x.Dispute != null ? x.Dispute.Id : null
                ))
                .ToListAsync(cancellationToken);

            return Ok(new PagedResponse<TransactionListItemDto>(items, page, pageSize, totalCount));
        }

        [HttpGet("{id:guid}")]
        public async Task<ActionResult<TransactionDetailDto>> GetTransactionById(
            Guid id,
            CancellationToken cancellationToken = default)
        {
            var transaction = await _db.Transactions
                .AsNoTracking()
                .Include(x => x.Dispute)
                .Where(x => x.Id == id)
                .Select(x => new TransactionDetailDto(
                    x.Id,
                    x.AccountId,
                    x.PostedAt,
                    x.Merchant,
                    x.Description,
                    x.Amount,
                    x.Currency,
                    x.Reference,
                    x.Dispute != null
                ))
                .FirstOrDefaultAsync(cancellationToken);

            if (transaction is null)
            {
                return NotFound(new { message = "Transaction not found." });
            }

            return Ok(transaction);
        }
    }
}