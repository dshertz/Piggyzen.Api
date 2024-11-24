using MediatR;
using Microsoft.EntityFrameworkCore;
using Piggyzen.Api.Data;

namespace Piggyzen.Api.Features.Transactions;

public class GetTransactionById
{
    // Query som tar emot ett Id som parameter
    public class Query : IRequest<Model>
    {
        public int Id { get; set; }
    }

    // Model som definierar resultatets struktur
    public class Model
    {
        public int Id { get; set; }
        public DateTime? BookingDate { get; set; } // Nullable BookingDate
        public DateTime TransactionDate { get; set; }
        public string Description { get; set; }
        public decimal Amount { get; set; }
        public decimal? Balance { get; set; } // Nullable Balance
        public string? CategoryName { get; set; }
        public string? ParentCategoryName { get; set; }
        public string? Memo { get; set; }
        public VerificationStatusEnum VerificationStatus { get; set; }
        public string VerificationStatusDisplay { get; set; }
        public CategorizationStatusEnum CategorizationStatus { get; set; }
        public string CategorizationStatusDisplay { get; set; }
        public TransactionTypeEnum TransactionType { get; set; }
    }

    // Handler som utför logiken och hämtar transaktionen från databasen
    public class Handler : IRequestHandler<Query, Model>
    {
        private readonly PiggyzenContext _context;

        public Handler(PiggyzenContext context)
        {
            _context = context;
        }

        public async Task<Model> Handle(Query request, CancellationToken cancellationToken)
        {
            // Hämta transaktionen från databasen
            var transaction = await _context.Transactions
                .Include(t => t.Category)
                .ThenInclude(c => c.ParentCategory)
                .Where(t => t.Id == request.Id)
                .SingleOrDefaultAsync(cancellationToken);

            if (transaction == null)
            {
                throw new KeyNotFoundException($"Transaction with ID {request.Id} not found.");
            }

            // Skapa modellen i minnet och beräkna display-fält
            return new Model
            {
                Id = transaction.Id,
                BookingDate = transaction.BookingDate,
                TransactionDate = transaction.TransactionDate,
                Description = transaction.Description,
                Amount = transaction.Amount,
                Balance = transaction.Balance,
                CategoryName = transaction.Category?.Name ?? string.Empty,
                ParentCategoryName = transaction.Category?.ParentCategory?.Name ?? string.Empty,
                Memo = transaction.Memo ?? string.Empty,
                VerificationStatus = transaction.VerificationStatus,
                VerificationStatusDisplay = Enum.GetName(typeof(VerificationStatusEnum), transaction.VerificationStatus) ?? "Unknown",
                CategorizationStatus = transaction.CategorizationStatus,
                CategorizationStatusDisplay = Enum.GetName(typeof(CategorizationStatusEnum), transaction.CategorizationStatus) ?? "Unknown",
                TransactionType = transaction.TransactionType,
            };
        }
    }
}