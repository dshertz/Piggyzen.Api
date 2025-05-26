using MediatR;
using Piggyzen.Api.Data;

namespace Piggyzen.Api.Features.Categories
{
    public class CreateCategory
    {
        public class Command : IRequest<Category>
        {
            public string Name { get; set; }
            public int? ParentCategoryId { get; set; }
            public bool IsActive { get; set; }
            public bool IsStandard { get; set; }
        }

        public class Handler : IRequestHandler<Command, Category>
        {
            private readonly PiggyzenContext _context;

            public Handler(PiggyzenContext context)
            {
                _context = context;
            }

            public async Task<Category> Handle(Command request, CancellationToken cancellationToken)
            {
                var category = new Category
                {
                    Name = request.Name,
                    ParentCategoryId = request.ParentCategoryId,
                    IsActive = request.IsActive,
                    IsStandard = request.IsStandard,
                    IsSystemCategory = false,
                    AllowSubcategories = false
                };

                _context.Categories.Add(category);
                await _context.SaveChangesAsync(cancellationToken);

                return category;
            }
        }
    }
}