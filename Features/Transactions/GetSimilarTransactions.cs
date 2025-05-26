using MediatR;
using Microsoft.EntityFrameworkCore;
using Piggyzen.Api.Data;
using Piggyzen.Api.Services;

public class GetSimilarTransactions
{
    public class Query : IRequest<List<TransactionResult>>
    {
        public int TransactionId { get; set; }
    }

    public class TransactionResult
    {
        public int Id { get; set; }
        public DateTime TransactionDate { get; set; }
        public string Description { get; set; }
        public decimal Amount { get; set; }
        public int? CategoryId { get; set; }
        public string? CategoryName { get; set; }
        public CategorizationStatusEnum CategorizationStatus { get; set; }
    }

    public class Handler : IRequestHandler<Query, List<TransactionResult>>
    {
        private readonly PiggyzenContext _context;
        private readonly TransactionService _transactionService;

        public Handler(PiggyzenContext context, TransactionService transactionService)
        {
            _context = context;
            _transactionService = transactionService;
        }

        public async Task<List<TransactionResult>> Handle(Query request, CancellationToken cancellationToken)
        {
            // Hämta den ursprungliga transaktionen
            var transaction = await _context.Transactions
                .Include(t => t.Category)
                .FirstOrDefaultAsync(t => t.Id == request.TransactionId, cancellationToken);

            if (transaction == null)
            {
                throw new KeyNotFoundException($"Transaction with ID {request.TransactionId} not found.");
            }

            // Hämta liknande transaktioner
            var similarTransactions = await _transactionService.FindSimilarTransactionsAsync(
                transaction.Description,
                request.TransactionId,
                cancellationToken
            );

            // Bygg resultatet
            var result = similarTransactions.Select(t => new TransactionResult
            {
                Id = t.Id,
                TransactionDate = t.TransactionDate,
                Description = t.Description,
                Amount = t.Amount,
                CategoryId = t.CategoryId,
                CategoryName = t.Category?.Name,
                CategorizationStatus = t.CategorizationStatus
            }).ToList();

            // Lägg till den ursprungliga transaktionen om den inte redan finns
            if (!result.Any(t => t.Id == transaction.Id))
            {
                result.Insert(0, new TransactionResult
                {
                    Id = transaction.Id,
                    TransactionDate = transaction.TransactionDate,
                    Description = transaction.Description,
                    Amount = transaction.Amount,
                    CategoryId = transaction.CategoryId,
                    CategoryName = transaction.Category?.Name,
                    CategorizationStatus = transaction.CategorizationStatus
                });
            }

            return result;
        }
    }
}