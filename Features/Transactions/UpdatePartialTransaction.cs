using MediatR;
using Piggyzen.Api.Data;
using Piggyzen.Api.Services;

public class UpdatePartialTransaction
{
    public class Command : IRequest<Transaction>
    {
        public int Id { get; set; }
        public DateTime TransactionDate { get; set; }
        public string Description { get; set; }
        public decimal Amount { get; set; }
        public int? CategoryId { get; set; }
        public string? Memo { get; set; }
    }

    public class Handler : IRequestHandler<Command, Transaction>
    {
        private readonly PiggyzenContext _context;
        private readonly TransactionService _transactionService;
        private readonly CategoryService _categoryService;

        public Handler(PiggyzenContext context, TransactionService transactionService, CategoryService categoryService)
        {
            _context = context;
            _transactionService = transactionService;
            _categoryService = categoryService;
        }

        public async Task<Transaction> Handle(Command request, CancellationToken cancellationToken)
        {
            var transaction = await _context.Transactions.FindAsync(request.Id);

            if (transaction == null)
                throw new KeyNotFoundException($"Transaction with ID {request.Id} not found.");

            // Uppdatera grundläggande fält
            transaction.TransactionDate = request.TransactionDate;
            transaction.Description = request.Description;
            transaction.Amount = request.Amount;
            transaction.Memo = request.Memo;

            // Hantera kategorisering via CategoryService
            await _categoryService.AssignOrSuggestCategoryAsync(transaction, request.CategoryId);

            // Generera nytt ReferenceId
            transaction.ReferenceId = _transactionService.GenerateReferenceId(
                transaction.BookingDate,
                request.TransactionDate,
                request.Description,
                request.Amount,
                transaction.Balance
            );

            await _context.SaveChangesAsync(cancellationToken);
            return transaction;
        }
    }
}