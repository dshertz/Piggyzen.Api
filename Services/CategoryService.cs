using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Piggyzen.Api.Data;

namespace Piggyzen.Api.Services
{
    public class CategoryService
    {
        private readonly PiggyzenContext _context;

        public CategoryService(PiggyzenContext context)
        {
            _context = context;
        }

        // Föreslår kategori baserat på transaktionsbeskrivning
        public async Task<int?> SuggestCategoryIdAsync(string description)
        {
            var query = @"
        SELECT CategoryId 
        FROM Transactions 
        WHERE @Description LIKE '%' || Description || '%' 
        GROUP BY CategoryId 
        ORDER BY COUNT(CategoryId) DESC, MAX(Id) DESC
        LIMIT 1;
    ";

            using var connection = new SqliteConnection(_context.Database.GetDbConnection().ConnectionString);
            using var command = new SqliteCommand(query, connection);
            command.Parameters.AddWithValue("@Description", description);

            await connection.OpenAsync();
            var result = await command.ExecuteScalarAsync();

            // Hantera DBNull och null-resultat
            if (result == DBNull.Value || result == null)
            {
                return null; // Ingen matchande kategori hittad
            }

            return Convert.ToInt32(result); // Returnera CategoryId
        }


        // Tilldelar kategori till en transaktion, med historik om ny
        public async Task AssignCategoryAsync(Transaction transaction, int categoryId)
        {
            transaction.CategoryId = categoryId;
            transaction.CategorizationStatus = CategorizationStatusEnum.ManuallyCategorized;

            bool alreadyExists = await _context.CategorizationHistory
                .AnyAsync(h => h.Description == transaction.Description && h.CategoryId == categoryId);

            if (!alreadyExists)
            {
                var categorizationEntry = new CategorizationHistory
                {
                    Description = transaction.Description,
                    CategoryId = categoryId
                };

                _context.CategorizationHistory.Add(categorizationEntry);
            }

            await _context.SaveChangesAsync();
        }

        // Ny metod för att hantera kategorisering och föreslå eller tilldela en kategori
        public async Task AssignOrSuggestCategoryAsync(Transaction transaction, int? categoryId)
        {
            if (categoryId.HasValue)
            {
                await AssignCategoryAsync(transaction, categoryId.Value);
            }
            else
            {
                var suggestedCategoryId = await SuggestCategoryIdAsync(transaction.Description);
                transaction.CategoryId = suggestedCategoryId;
                transaction.CategorizationStatus = suggestedCategoryId.HasValue
                    ? CategorizationStatusEnum.AutoCategorized
                    : CategorizationStatusEnum.NotCategorized;
            }
        }
    }
}