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
        public ICollection<Category> Subcategories { get; set; } = new List<Category>();
        public ICollection<Transaction> Transactions { get; set; } = new List<Transaction>();
        public CategoryGroupEnum Group { get; set; } = CategoryGroupEnum.None;
    }
}