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
            public bool IsActive { get; set; } // Nytt fält för IsActive
            public bool IsSystemCategory { get; set; } // Nytt fält för IsSystemCategory
            public bool AllowSubcategories { get; set; }
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
                // Hämtar alla kategorier med deras parent-category och fält
                var categories = await _context.Categories
                    .Include(c => c.ParentCategory) // Inkludera ParentCategory för att få ParentCategoryName
                    .Select(c => new Result
                    {
                        Id = c.Id,
                        Name = c.Name,
                        ParentCategoryId = c.ParentCategoryId,
                        ParentCategoryName = c.ParentCategory != null ? c.ParentCategory.Name : null,
                        IsActive = c.IsActive, // Lägg till IsActive
                        IsSystemCategory = c.IsSystemCategory, // Lägg till IsSystemCategory
                        AllowSubcategories = c.AllowSubcategories

                    })
                    .ToListAsync(cancellationToken);

                return categories;
            }
        }
    }
}