using MediatR;
using Microsoft.EntityFrameworkCore;
using Piggyzen.Api.Data;

namespace Piggyzen.Api.Features.Categories
{
    public class UpdateCategory
    {
        public class Command : IRequest
        {
            public int Id { get; set; }
            public string Name { get; set; }
            public int? ParentCategoryId { get; set; }
            public bool IsActive { get; set; } // Möjlighet att ändra IsActive
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
                var existingCategory = await _context.Categories
                    .Include(c => c.Subcategories)
                    .FirstOrDefaultAsync(c => c.Id == request.Id, cancellationToken);

                if (existingCategory == null)
                    throw new KeyNotFoundException($"Category with ID {request.Id} not found.");

                // Förhindra ändringar av CoreCategory
                if (existingCategory.IsSystemCategory)
                    throw new InvalidOperationException($"Category with ID {request.Id} is a core category and cannot be modified.");

                // Uppdatera fält som kan ändras
                existingCategory.Name = request.Name;
                existingCategory.ParentCategoryId = request.ParentCategoryId;
                existingCategory.IsActive = request.IsActive;

                // Förhindra cirkulära referenser
                if (request.ParentCategoryId == request.Id)
                    throw new InvalidOperationException("A category cannot be its own parent.");

                await _context.SaveChangesAsync(cancellationToken);
            }
        }
    }
}