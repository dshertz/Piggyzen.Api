using MediatR;
using Microsoft.EntityFrameworkCore;
using Piggyzen.Api.Data;

namespace Piggyzen.Api.Features.Transactions
{
    public class GetAllTransactions
    {
        // Query som skickar en förfrågan för att hämta alla transaktioner
        public class Query : IRequest<List<Result>> { }

        // Result-klass som efterliknar strukturen i GetAllTransactionsDto
        public class Result
        {
            public int Id { get; set; }
            public DateTime TransactionDate { get; set; }
            public string Description { get; set; }
            public decimal Amount { get; set; }
            public decimal? Balance { get; set; } // Hanterar nullable Balance
            public int? CategoryId { get; set; }
            public string? CategoryName { get; set; }
            public string? Memo { get; set; }
            public CategorizationStatusEnum CategorizationStatus { get; set; }
            public string CategorizationStatusDisplay { get; set; }
            public VerificationStatusEnum VerificationStatus { get; set; } // Lägg till VerificationStatus
            public TransactionTypeEnum TransactionType { get; set; }
        }

        // Handler för att hämta och forma resultatet
        public class Handler : IRequestHandler<Query, List<Result>>
        {
            private readonly PiggyzenContext _context;

            public Handler(PiggyzenContext context)
            {
                _context = context;
            }

            public async Task<List<Result>> Handle(Query request, CancellationToken cancellationToken)
            {
                // Hämta data från databasen utan att använda Enum.GetName
                var transactions = await _context.Transactions
                    .Include(t => t.Category)
                    .OrderBy(t => t.TransactionDate)
                    .ThenBy(t => t.Id)
                    .Select(t => new
                    {
                        t.Id,
                        t.TransactionDate,
                        t.Description,
                        t.Amount,
                        t.Balance,
                        t.CategoryId,
                        CategoryName = t.Category != null ? t.Category.Name : null,
                        t.Memo,
                        t.CategorizationStatus,
                        t.VerificationStatus,
                        t.TransactionType
                    })
                    .ToListAsync(cancellationToken);

                // Mappa till Result-objekt i minnet
                return transactions.Select(t => new Result
                {
                    Id = t.Id,
                    TransactionDate = t.TransactionDate,
                    Description = t.Description,
                    Amount = t.Amount,
                    Balance = t.Balance,
                    CategoryId = t.CategoryId,
                    CategoryName = t.CategoryName,
                    Memo = t.Memo,
                    CategorizationStatus = t.CategorizationStatus,
                    CategorizationStatusDisplay = Enum.GetName(typeof(CategorizationStatusEnum), t.CategorizationStatus) ?? "Unknown",
                    VerificationStatus = t.VerificationStatus,
                    TransactionType = t.TransactionType
                }).ToList();
            }
        }
    }
}