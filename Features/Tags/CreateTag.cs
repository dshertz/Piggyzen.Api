using MediatR;
using Piggyzen.Api.Data;
using Piggyzen.Api.Models;

namespace Piggyzen.Api.Features.Tags
{
    public class CreateTag
    {
        public class Command : IRequest<Tag>
        {
            public string Name { get; set; }
            public TagTypeEnum Type { get; set; }

        }

        public class Handler : IRequestHandler<Command, Tag>
        {
            private readonly PiggyzenContext _context;

            public Handler(PiggyzenContext context)
            {
                _context = context;
            }

            public async Task<Tag> Handle(Command request, CancellationToken cancellationToken)
            {
                var tag = new Tag
                {
                    Name = request.Name,
                    Type = request.Type
                };

                _context.Tags.Add(tag);
                await _context.SaveChangesAsync(cancellationToken);

                return tag;
            }
        }
    }
}