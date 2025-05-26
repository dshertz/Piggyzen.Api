using MediatR;
using Microsoft.EntityFrameworkCore;
using Piggyzen.Api.Data;
using Piggyzen.Api.Services;

namespace Piggyzen.Api.Features.Transactions
{
    public class GetAllTransactions
    {
        public class Query : IRequest<List<Result>> { }

        public class Result
        {
            public int Id { get; set; }
            public DateTime TransactionDate { get; set; }
            public string Description { get; set; }
            public decimal Amount { get; set; }
            public decimal? Balance { get; set; }
            public int? CategoryId { get; set; }
            public string? CategoryName { get; set; }
            public string? Memo { get; set; }
            public CategorizationStatusEnum CategorizationStatus { get; set; }
            public string CategorizationStatusDisplay { get; set; }
            public VerificationStatusEnum VerificationStatus { get; set; }
            public TransactionTypeEnum TransactionType { get; set; }
            public bool HasSimilar { get; set; }
        }

        public class Handler : IRequestHandler<Query, List<Result>>
        {
            private readonly PiggyzenContext _context;
            private readonly TransactionService _transactionService;

            public Handler(PiggyzenContext context, TransactionService transactionService)
            {
                _context = context;
                _transactionService = transactionService;
            }

            public async Task<List<Result>> Handle(Query request, CancellationToken cancellationToken)
            {
                // Uppdatera HasSimilar-flaggan innan hämtning
                await _transactionService.UpdateHasSimilarFlagsAsync(cancellationToken);

                // Hämta data från databasen
                var transactions = await _context.Transactions
                    .Include(t => t.Category)
                    .OrderBy(t => t.TransactionDate)
                    .ThenBy(t => t.Id)
                    .ToListAsync(cancellationToken);

                // Returnera transaktionerna direkt till klienten
                var results = transactions.Select(t => new Result
                {
                    Id = t.Id,
                    TransactionDate = t.TransactionDate,
                    Description = t.Description,
                    Amount = t.Amount,
                    Balance = t.Balance,
                    CategoryId = t.CategoryId,
                    CategoryName = t.Category?.Name,
                    Memo = t.Memo,
                    CategorizationStatus = t.CategorizationStatus,
                    CategorizationStatusDisplay = Enum.GetName(typeof(CategorizationStatusEnum), t.CategorizationStatus) ?? "Unknown",
                    VerificationStatus = t.VerificationStatus,
                    TransactionType = t.TransactionType,
                    HasSimilar = t.HasSimilar
                }).ToList();

                return results;
            }
        }
    }
}