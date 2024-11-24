using MediatR;
using Piggyzen.Api.Data;

namespace Piggyzen.Api.Features.Transactions
{
    public class ApproveCategory
    {
        public class Command : IRequest
        {
            public int TransactionId { get; set; }
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
                var transaction = await _context.Transactions.FindAsync(request.TransactionId);
                if (transaction == null)
                {
                    throw new KeyNotFoundException($"Transaction with ID {request.TransactionId} not found.");
                }

                transaction.CategorizationStatus = CategorizationStatusEnum.ManuallyCategorized;
                await _context.SaveChangesAsync(cancellationToken);
            }
        }
    }
}