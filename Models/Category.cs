namespace Piggyzen.Api.Models
{
    public class Category
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int? ParentCategoryId { get; set; }
        public Category ParentCategory { get; set; }
        public bool IsStandard { get; set; }
        public bool IsActive { get; set; }
        public bool IsSystemCategory { get; set; }
        public bool AllowSubcategories { get; set; }
        public ICollection<Category> Subcategories { get; set; } = new List<Category>();
        public ICollection<Transaction> Transactions { get; set; } = new List<Transaction>();
    }
}