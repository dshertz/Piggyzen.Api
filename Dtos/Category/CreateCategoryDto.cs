namespace piggyzen.api.Dtos.Category
{
    public class CreateCategoryDto
    {
        public string Name { get; set; }
        public int? ParentCategoryId { get; set; }
    }
}