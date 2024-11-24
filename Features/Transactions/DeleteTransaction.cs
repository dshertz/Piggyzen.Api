using MediatR;
using Microsoft.EntityFrameworkCore;
using Piggyzen.Api.Data;

namespace Piggyzen.Api.Features.Transactions
{
    public class DeleteTransaction
    {
        public class Command : IRequest
        {
            public int Id { get; set; }
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
                // Hämta transaktionen
                var transaction = await _context.Transactions.FindAsync(request.Id);

                if (transaction == null)
                {
                    throw new KeyNotFoundException($"Transaction with ID {request.Id} could not be found in the database.");
                }

                // Ta bort transaktionen
                _context.Transactions.Remove(transaction);

                // Spara ändringarna och kontrollera att borttagningen lyckades
                var result = await _context.SaveChangesAsync(cancellationToken);

                if (result == 0)
                {
                    throw new DbUpdateException("Failed to delete the transaction. No changes were saved.");
                }
            }
        }
    }
}