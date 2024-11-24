using MediatR;
using Microsoft.EntityFrameworkCore;
using Piggyzen.Api.Data;

namespace Piggyzen.Api.Features.Tags
{
    public class GetAllTags
    {
        public class Query : IRequest<List<Result>> { }

        public class Result
        {
            public int Id { get; set; }
            public string Name { get; set; }
            public string Type { get; set; }
        }

        public class Handler : IRequestHandler<Query, List<Result>>
        {
            private readonly PiggyzenContext _context;

            public Handler(PiggyzenContext context)
            {
                _context = context;
            }

            public async Task<List<Result>> Handle(Query request, CancellationToken cancellationToken)
            {
                return await _context.Tags
                    .Select(t => new Result
                    {
                        Id = t.Id,
                        Name = t.Name,
                        Type = t.Type.ToString()
                    })
                    .ToListAsync(cancellationToken);
            }
        }
    }
}