using MediatR;
using Piggyzen.Api.Data;
using Piggyzen.Api.Services;

public class UpdateCompleteTransaction
{
    public class Command : IRequest
    {
        public int Id { get; set; }
        public int? CategoryId { get; set; }
        public string? Memo { get; set; }
    }

    public class Handler : IRequestHandler<Command>
    {
        private readonly PiggyzenContext _context;
        private readonly CategoryService _categoryService;

        public Handler(PiggyzenContext context, CategoryService categoryService)
        {
            _context = context;
            _categoryService = categoryService;
        }

        public async Task Handle(Command request, CancellationToken cancellationToken)
        {
            var transaction = await _context.Transactions.FindAsync(request.Id);

            if (transaction == null)
                throw new KeyNotFoundException($"Transaction with ID {request.Id} not found.");

            // Hantera kategorisering via CategoryService
            if (request.CategoryId.HasValue)
            {
                await _categoryService.AssignCategoryAsync(transaction, request.CategoryId.Value);
            }
            else
            {
                transaction.CategorizationStatus = CategorizationStatusEnum.NotCategorized;
            }

            // Uppdatera memo
            transaction.Memo = request.Memo;

            await _context.SaveChangesAsync(cancellationToken);
        }
    }
}