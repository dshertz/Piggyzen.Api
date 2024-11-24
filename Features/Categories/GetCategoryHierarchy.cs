using MediatR;
using Microsoft.EntityFrameworkCore;
using Piggyzen.Api.Data;

namespace Piggyzen.Api.Features.Categories
{
    public class GetCategoryHierarchy
    {
        public class Query : IRequest<List<Model>> { }

        public class Model
        {
            public int Id { get; set; }
            public string Name { get; set; }
            public int? ParentCategoryId { get; set; }
            public List<Model> Subcategories { get; set; } = new();
        }

        public class Handler : IRequestHandler<Query, List<Model>>
        {
            private readonly PiggyzenContext _context;

            public Handler(PiggyzenContext context)
            {
                _context = context;
            }

            public async Task<List<Model>> Handle(Query request, CancellationToken cancellationToken)
            {
                // Hämta alla kategorier från databasen
                var allCategories = await _context.Categories
                    .Where(c => c.IsActive) // Bara aktiva kategorier
                    .Select(c => new Model
                    {
                        Id = c.Id,
                        Name = c.Name,
                        ParentCategoryId = c.ParentCategoryId
                    })
                    .ToListAsync(cancellationToken);

                // Skapa en lookup-tabell
                var categoryDictionary = allCategories.ToDictionary(c => c.Id);

                // Bygg hierarkin
                foreach (var category in allCategories)
                {
                    if (category.ParentCategoryId.HasValue &&
                        categoryDictionary.TryGetValue(category.ParentCategoryId.Value, out var parentCategory))
                    {
                        parentCategory.Subcategories.Add(category);
                    }
                }

                // Returnera bara root-kategorierna
                return allCategories.Where(c => c.ParentCategoryId == null).ToList();
            }
        }
    }
}