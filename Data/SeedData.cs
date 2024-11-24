using System.Text.Json;

namespace Piggyzen.Api.Data
{
    public class SeedData
    {
        public static async Task LoadCategoryData(PiggyzenContext context)
        {
            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };

            if (!context.Categories.Any())
            {
                var json = System.IO.File.ReadAllText("Data/Json/categories.json");
                var categories = JsonSerializer.Deserialize<List<Category>>(json, options);

                if (categories is not null && categories.Count > 0)
                {
                    await context.Categories.AddRangeAsync(categories);
                    await context.SaveChangesAsync();
                }
            }
        }

        public static async Task LoadTagData(PiggyzenContext context)
        {
            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };

            if (!context.Tags.Any())
            {
                var json = System.IO.File.ReadAllText("Data/Json/tags.json");
                var tags = JsonSerializer.Deserialize<List<Tag>>(json, options);

                if (tags is not null && tags.Count > 0)
                {
                    await context.Tags.AddRangeAsync(tags);
                    await context.SaveChangesAsync();
                }
            }
        }

        public static async Task SeedAll(PiggyzenContext context)
        {
            await LoadCategoryData(context);
            await LoadTagData(context);
        }
    }
}