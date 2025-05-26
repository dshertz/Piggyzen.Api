using MediatR;
using Microsoft.EntityFrameworkCore;
using Piggyzen.Api.Data;

namespace Piggyzen.Api.Features.Transactions
{
    public class ApproveCategory
    {
        public class Command : IRequest
        {
            public List<int> TransactionIds { get; set; } = new();
        }

        public class Handler : IRequestHandler<Command>
        {
            private readonly PiggyzenContext _context;

            public Handler(PiggyzenContext context)
            {
                _context = context;
            }

            public async Task Handle(Command request, CancellationToken cancellationToken)
            {
                // Hämta alla transaktioner som matchar de skickade IDs
                var transactions = await _context.Transactions
                    .Where(t => request.TransactionIds.Contains(t.Id))
                    .ToListAsync(cancellationToken);

                if (!transactions.Any())
                {
                    throw new KeyNotFoundException("No transactions found for the given IDs.");
                }

                foreach (var transaction in transactions)
                {
                    // Uppdatera status till ManuallyCategorized
                    transaction.CategorizationStatus = CategorizationStatusEnum.ManuallyCategorized;

                    // Sätt HasSimilar till false
                    transaction.HasSimilar = false;
                }

                // Spara alla ändringar i en enda operation
                await _context.SaveChangesAsync(cancellationToken);
            }
        }
    }
}