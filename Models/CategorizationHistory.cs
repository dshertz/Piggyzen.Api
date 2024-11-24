namespace Piggyzen.Api.Models
{
    public class CategorizationHistory
    {
        public int Id { get; set; }
        public string Description { get; set; }
        public int CategoryId { get; set; }

        // Navigation properties
        public Category Category { get; set; }
    }
}