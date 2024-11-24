using MediatR;
using Microsoft.EntityFrameworkCore;
using Piggyzen.Api.Data;

namespace Piggyzen.Api.Features.Tags
{
    public class GetTagById
    {
        public class Query : IRequest<Result>
        {
            public int Id { get; set; }
        }

        public class Result
        {
            public int Id { get; set; }
            public string Name { get; set; }
            public int Type { get; set; }
        }

        public class Handler : IRequestHandler<Query, Result>
        {
            private readonly PiggyzenContext _context;

            public Handler(PiggyzenContext context)
            {
                _context = context;
            }

            public async Task<Result> Handle(Query request, CancellationToken cancellationToken)
            {
                var tag = await _context.Tags
                    .Where(t => t.Id == request.Id)
                    .Select(t => new Result
                    {
                        Id = t.Id,
                        Name = t.Name,
                        Type = (int)t.Type
                    })
                    .SingleOrDefaultAsync(cancellationToken);

                return tag ?? throw new KeyNotFoundException($"Tag with ID {request.Id} not found.");
            }
        }
    }
}