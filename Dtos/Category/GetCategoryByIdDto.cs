namespace piggyzen.api.Dtos.Category
{
    public class GetCategoryByIdDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int? ParentCategoryId { get; set; }
        public string ParentCategoryName { get; set; } // Exponerar förälderns namn
        public List<GetAllCategoriesDto> Subcategories { get; set; } = new();
    }
}