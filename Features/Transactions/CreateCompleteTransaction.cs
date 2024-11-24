using MediatR;
using Microsoft.EntityFrameworkCore;
using Piggyzen.Api.Data;
using Piggyzen.Api.Services;

namespace Piggyzen.Api.Features.Transactions
{
    public class CreateCompleteTransaction
    {
        public class Command : IRequest<Transaction>
        {
            public DateTime BookingDate { get; set; }
            public DateTime TransactionDate { get; set; }
            public string Description { get; set; }
            public decimal Amount { get; set; }
            public decimal Balance { get; set; }
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
                var referenceId = _transactionService.GenerateReferenceId(
                    request.BookingDate,
                    request.TransactionDate,
                    request.Description,
                    request.Amount,
                    request.Balance
                );

                // Kontrollera om transaktionen redan existerar
                var existingTransaction = await _context.Transactions
                    .SingleOrDefaultAsync(t => t.ReferenceId == referenceId, cancellationToken);

                if (existingTransaction != null)
                    throw new InvalidOperationException("A transaction with this ReferenceId already exists.");

                // Skapa en ny transaktion
                var transaction = new Transaction
                {
                    BookingDate = request.BookingDate,
                    TransactionDate = request.TransactionDate,
                    Description = request.Description?.Trim() ?? string.Empty,
                    Amount = request.Amount,
                    Balance = request.Balance,
                    ReferenceId = referenceId,
                    CategoryId = request.CategoryId,
                    TransactionType = TransactionTypeEnum.Complete // Sätt TransactionType
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