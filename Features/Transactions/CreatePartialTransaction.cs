using MediatR;
using Microsoft.EntityFrameworkCore;
using Piggyzen.Api.Data;
using Piggyzen.Api.Services;

namespace Piggyzen.Api.Features.Transactions
{
    public class CreatePartialTransaction
    {
        public class Command : IRequest<Transaction>
        {
            public DateTime TransactionDate { get; set; }
            public string Description { get; set; }
            public decimal Amount { get; set; }
            public int? CategoryId { get; set; }
        }

        public class Handler : IRequestHandler<Command, Transaction>
        {
            private readonly PiggyzenContext _context;
            private readonly CategoryService _categoryService;
            private readonly TransactionService _transactionService;

            public Handler(PiggyzenContext context, CategoryService categoryService, TransactionService transactionService)
            {
                _context = context;
                _categoryService = categoryService;
                _transactionService = transactionService;
            }

            public async Task<Transaction> Handle(Command request, CancellationToken cancellationToken)
            {
                // Generera ReferenceId baserat på ofullständig data
                var referenceId = _transactionService.GenerateReferenceId(
                    null, // Ingen BookingDate
                    request.TransactionDate,
                    request.Description,
                    request.Amount,
                    null // Ingen Balance
                );

                // Kontrollera potentiella dubletter
                var existingTransaction = await _context.Transactions
                    .FirstOrDefaultAsync(t => t.ReferenceId == referenceId, cancellationToken);

                if (existingTransaction != null)
                {
                    // Flagga som misstänkt dublett och kasta ett undantag
                    throw new InvalidOperationException(
                        "A transaction with similar details already exists. Please review potential duplicates.");
                }

                // Skapa ny transaktion
                var transaction = new Transaction
                {
                    BookingDate = null, // Ingen BookingDate
                    TransactionDate = request.TransactionDate,
                    Description = request.Description?.Trim() ?? string.Empty,
                    Amount = request.Amount,
                    Balance = null, // Ingen Balance
                    ReferenceId = referenceId,
                    CategoryId = request.CategoryId,
                    VerificationStatus = VerificationStatusEnum.FlaggedForReview, // Markera som misstänkt dublett
                    TransactionType = TransactionTypeEnum.Partial // Sätt TransactionType
                };

                // Tilldela eller föreslå kategori
                await _categoryService.AssignOrSuggestCategoryAsync(transaction, request.CategoryId);

                // Lägg till transaktionen i databasen och spara
                _context.Transactions.Add(transaction);
                await _context.SaveChangesAsync(cancellationToken);

                return transaction;
            }
        }
    }
}