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

                existingCategory.Name = request.Name;
                existingCategory.ParentCategoryId = request.ParentCategoryId;

                // Prevent circular references if necessary
                if (request.ParentCategoryId == request.Id)
                    throw new InvalidOperationException("A category cannot be its own parent.");

                await _context.SaveChangesAsync(cancellationToken);
            }
        }
    }
}