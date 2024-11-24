namespace Piggyzen.Api.Models
{
    public class Transaction
    {
        public int Id { get; set; }
        public DateTime? BookingDate { get; set; }
        public DateTime TransactionDate { get; set; }
        public string Description { get; set; }
        public decimal Amount { get; set; }
        public decimal? Balance { get; set; }
        public int? CategoryId { get; set; }
        public Category? Category { get; set; }
        public string? ReferenceId { get; set; }
        public string? Memo { get; set; }

        // Självreferens för splits
        public int? ParentTransactionId { get; set; }
        public Transaction? ParentTransaction { get; set; }
        public List<Transaction> ChildTransactions { get; set; } = new();

        // Koppling till TransactionRelation
        public List<TransactionRelation> SourceRelations { get; set; } = new(); // Relationer där denna transaktion är källan
        public List<TransactionRelation> TargetRelations { get; set; } = new(); // Relationer där denna transaktion är målet

        public TransactionTypeEnum TransactionType { get; set; } = TransactionTypeEnum.Complete; // Standard till Complete
        public bool IsOutlayOrReturn { get; set; } = false;
        public decimal? AdjustedAmount { get; set; }
        public CategorizationStatusEnum CategorizationStatus { get; set; } = CategorizationStatusEnum.NotCategorized;
        public VerificationStatusEnum VerificationStatus { get; set; } = VerificationStatusEnum.Unverified; // Verifieringsstatus. Verifiera Misstänkta Dubletter eller Unik
        public ICollection<TransactionTag> TransactionTags { get; set; } = new List<TransactionTag>();
    }
}
