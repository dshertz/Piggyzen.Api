using MediatR;
using Microsoft.EntityFrameworkCore;
using Piggyzen.Api.Data;

public class UpdateVerificationStatus
{
    public class Command : IRequest
    {
        public int TransactionId { get; set; }
        public VerificationStatusEnum VerificationStatus { get; set; }
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
            var transaction = await _context.Transactions
                .FirstOrDefaultAsync(t => t.Id == request.TransactionId, cancellationToken);

            if (transaction == null)
                throw new KeyNotFoundException($"Transaction with ID {request.TransactionId} not found.");

            // Uppdatera verifieringsstatus
            transaction.VerificationStatus = request.VerificationStatus;

            await _context.SaveChangesAsync(cancellationToken);
        }
    }
}