using MediatR;
using Microsoft.EntityFrameworkCore;
using Piggyzen.Api.Data;

namespace Piggyzen.Api.Features.Categories
{
    public class GetAllCategories
    {
        // Query för att hämta alla kategorier
        public class Query : IRequest<List<Result>> { }

        // Result-klass som efterliknar strukturen i GetAllCategoriesDto
        public class Result
        {
            public int Id { get; set; }
            public string Name { get; set; }
            public int? ParentCategoryId { get; set; }
            public string? ParentCategoryName { get; set; }
            public List<Result> Subcategories { get; set; } = new List<Result>();
        }

        // Handler som hanterar förfrågan och formar resultatet
        public class Handler : IRequestHandler<Query, List<Result>>
        {
            private readonly PiggyzenContext _context;

            public Handler(PiggyzenContext context)
            {
                _context = context;
            }

            public async Task<List<Result>> Handle(Query request, CancellationToken cancellationToken)
            {
                var categories = await _context.Categories
                .Include(c => c.ParentCategory) // Inkludera ParentCategory för att få ParentCategoryName
                .Select(c => new Result
                {
                    Id = c.Id,
                    Name = c.Name,
                    ParentCategoryName = c.ParentCategory != null ? c.ParentCategory.Name : null
                })
                .ToListAsync(cancellationToken);

                return categories;
            }
        }
    }
}