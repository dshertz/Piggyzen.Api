using MediatR;
using Microsoft.EntityFrameworkCore;
using Piggyzen.Api.Data;
using Piggyzen.Api.Services;

namespace Piggyzen.Api.Features.Transactions
{
    public class CategorizeUncategorizedTransactions
    {
        public class Command : IRequest<Result> { }

        public class Result
        {
            public int TotalProcessed { get; set; }
            public int AutoCategorized { get; set; }
            public int NotCategorized { get; set; }
        }

        public class Handler : IRequestHandler<Command, Result>
        {
            private readonly PiggyzenContext _context;
            private readonly CategoryService _categoryService;

            public Handler(PiggyzenContext context, CategoryService categoryService)
            {
                _context = context;
                _categoryService = categoryService;
            }

            public async Task<Result> Handle(Command request, CancellationToken cancellationToken)
            {
                var transactions = await _context.Transactions
                    .Where(t => !t.CategoryId.HasValue) // Hämta transaktioner utan kategori
                    .ToListAsync(cancellationToken);

                int autoCategorized = 0;
                int notCategorized = 0;

                foreach (var transaction in transactions)
                {
                    var suggestedCategoryId = await _categoryService.SuggestCategoryIdAsync(transaction.Description);
                    if (suggestedCategoryId.HasValue)
                    {
                        transaction.CategoryId = suggestedCategoryId;
                        transaction.CategorizationStatus = CategorizationStatusEnum.AutoCategorized;
                        autoCategorized++;
                    }
                    else
                    {
                        transaction.CategorizationStatus = CategorizationStatusEnum.NotCategorized;
                        notCategorized++;
                    }
                }

                await _context.SaveChangesAsync(cancellationToken);

                var result = new Result
                {
                    TotalProcessed = transactions.Count,
                    AutoCategorized = autoCategorized,
                    NotCategorized = notCategorized
                };

                // Logga resultatet
                Console.WriteLine($"TotalProcessed: {result.TotalProcessed}, AutoCategorized: {result.AutoCategorized}, NotCategorized: {result.NotCategorized}");

                return result;
            }
        }
    }
}