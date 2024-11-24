namespace Piggyzen.Api.Models
{
    public class Tag
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public TagTypeEnum Type { get; set; } // Typen av tagg: Owner, Entity, Activity
        public ICollection<TransactionTag> TransactionTags { get; set; } = new List<TransactionTag>();
    }
}