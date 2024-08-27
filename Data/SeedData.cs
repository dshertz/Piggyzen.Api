using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using piggyzen.api.Models;

namespace piggyzen.api.Data
{
    public class SeedData
    {
        public static async Task LoadCategoryData(PiggyzenContext context)
        {
            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };

            if (context.Categories.Any()) return;

            var json = System.IO.File.ReadAllText("Data/Json/categories.json");
            var categories = JsonSerializer.Deserialize<List<Category>>(json, options);

            if (categories is not null && categories.Count > 0)
            {
                await context.Categories.AddRangeAsync(categories);
                await context.SaveChangesAsync();
            }
        }

        public static async Task LoadTransactionData(PiggyzenContext context)
        {
            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };

            if (context.Transactions.Any()) return;

            var json = System.IO.File.ReadAllText("Data/Json/transactions.json");
            var transactions = JsonSerializer.Deserialize<List<Transaction>>(json, options);

            if (transactions is not null && transactions.Count > 0)
            {
                await context.Transactions.AddRangeAsync(transactions);
                await context.SaveChangesAsync();
            }
        }
    }
}