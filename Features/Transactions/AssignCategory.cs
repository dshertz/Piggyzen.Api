using MediatR;
using Piggyzen.Api.Data;
using Piggyzen.Api.Services;

namespace Piggyzen.Api.Features.Transactions
{
    public class AssignCategory
    {
        public class Command : IRequest
        {
            public List<int> TransactionIds { get; set; } = new();
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
                if (request.TransactionIds == null || !request.TransactionIds.Any())
                {
                    Console.WriteLine("No transactions provided.");
                    throw new ArgumentException("No transactions provided for categorization.");
                }

                foreach (var transactionId in request.TransactionIds)
                {
                    Console.WriteLine($"Processing TransactionId: {transactionId}, CategoryId: {request.CategoryId}");
                    // Hämta transaktionen
                    var transaction = await _context.Transactions.FindAsync(transactionId);
                    if (transaction == null)
                    {
                        Console.WriteLine($"Transaction with ID {transactionId} not found. Skipping.");
                        continue; // Hoppa över om transaktionen inte finns
                    }

                    // Använd CategoryService för att tilldela kategorin och hantera historiken
                    await _categoryService.AssignCategoryAsync(transaction, request.CategoryId);
                }

                // Spara ändringarna efter att alla transaktioner bearbetats
                await _context.SaveChangesAsync(cancellationToken);
            }
        }
    }
}