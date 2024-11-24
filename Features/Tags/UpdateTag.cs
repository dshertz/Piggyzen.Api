using MediatR;
using Microsoft.EntityFrameworkCore;
using Piggyzen.Api.Data;

namespace Piggyzen.Api.Features.Tags
{
    public class UpdateTag
    {
        public class Command : IRequest
        {
            public int Id { get; set; }
            public string Name { get; set; }
            public TagTypeEnum Type { get; set; }
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

                tag.Name = request.Name;
                tag.Type = request.Type;

                await _context.SaveChangesAsync(cancellationToken);
            }
        }
    }
}