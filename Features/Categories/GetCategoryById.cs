using MediatR;
using Microsoft.EntityFrameworkCore;
using Piggyzen.Api.Data;

namespace Piggyzen.Api.Features.Categories
{
    public class GetCategoryById
    {
        public class Query : IRequest<Model>
        {
            public int Id { get; set; }
        }

        public class Model
        {
            public int Id { get; set; }
            public string Name { get; set; }
            public int? ParentCategoryId { get; set; }
            public string ParentCategoryName { get; set; } = string.Empty;
            public bool IsActive { get; set; } // Nytt fält
            public bool IsSystemCategory { get; set; } // Nytt fält
            public List<SubcategoryModel> Subcategories { get; set; } = new();

            public class SubcategoryModel
            {
                public int Id { get; set; }
                public string Name { get; set; }
                public int? ParentCategoryId { get; set; }
                public bool IsActive { get; set; } // Nytt fält
                public bool IsSystemCategory { get; set; } // Nytt fält
            }
        }

        public class Handler : IRequestHandler<Query, Model>
        {
            private readonly PiggyzenContext _context;

            public Handler(PiggyzenContext context)
            {
                _context = context;
            }

            public async Task<Model> Handle(Query request, CancellationToken cancellationToken)
            {
                var category = await _context.Categories
                    .Include(c => c.ParentCategory)
                    .Include(c => c.Subcategories)
                    .Where(c => c.Id == request.Id)
                    .Select(c => new Model
                    {
                        Id = c.Id,
                        Name = c.Name,
                        ParentCategoryId = c.ParentCategoryId,
                        ParentCategoryName = c.ParentCategory != null ? c.ParentCategory.Name : string.Empty,
                        IsActive = c.IsActive, // Lägg till IsActive
                        IsSystemCategory = c.IsSystemCategory, // Lägg till IsSystemCategory
                        Subcategories = c.Subcategories.Select(sc => new Model.SubcategoryModel
                        {
                            Id = sc.Id,
                            Name = sc.Name,
                            ParentCategoryId = sc.ParentCategoryId,
                            IsActive = sc.IsActive, // Lägg till IsActive
                            IsSystemCategory = sc.IsSystemCategory // Lägg till IsSystemCategory
                        }).ToList()
                    })
                    .SingleOrDefaultAsync(cancellationToken);

                return category ?? throw new KeyNotFoundException($"Category with ID {request.Id} not found.");
            }
        }
    }
}