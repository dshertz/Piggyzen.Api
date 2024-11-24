using MediatR;
using Piggyzen.Api.Data;

namespace Piggyzen.Api.Features.Categories
{
    public class DeleteCategory
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
                var category = await _context.Categories.FindAsync(request.Id);
                if (category == null)
                {
                    throw new KeyNotFoundException($"Category with ID {request.Id} not found.");
                }

                _context.Categories.Remove(category);
                await _context.SaveChangesAsync(cancellationToken);
            }
        }
    }
}