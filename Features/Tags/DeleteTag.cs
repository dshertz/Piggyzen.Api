using MediatR;
using Piggyzen.Api.Data;

namespace Piggyzen.Api.Features.Tags
{
    public class DeleteTag
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
                var tag = await _context.Tags.FindAsync(request.Id);
                if (tag == null)
                    throw new KeyNotFoundException($"Tag with ID {request.Id} not found.");

                _context.Tags.Remove(tag);
                await _context.SaveChangesAsync(cancellationToken);
            }
        }
    }
}