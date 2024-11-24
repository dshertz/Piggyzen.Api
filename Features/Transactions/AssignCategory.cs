using MediatR;
using Piggyzen.Api.Data;
using Piggyzen.Api.Services;

namespace Piggyzen.Api.Features.Transactions
{
    public class AssignCategory
    {
        public class Command : IRequest
        {
            public int TransactionId { get; set; }
            public int CategoryId { get; set; }
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
                Console.WriteLine($"TransactionId: {request.TransactionId}, CategoryId: {request.CategoryId}");
                // Hämta transaktionen
                var transaction = await _context.Transactions.FindAsync(request.TransactionId);
                if (transaction == null)
                {
                    Console.WriteLine("Transaction not found.");
                    throw new KeyNotFoundException($"Transaction with ID {request.TransactionId} not found.");
                }
                // Använd CategoryService för att tilldela kategorin och hantera historiken
                await _categoryService.AssignCategoryAsync(transaction, request.CategoryId);
            }
        }
    }
}