namespace Piggyzen.Api.Models
{
    public class TransactionRelation
    {
        public int Id { get; set; } // Primärnyckel
        public int SourceTransactionId { get; set; } // Transaktionen som orsakar relationen (t.ex. återköpet)
        public Transaction SourceTransaction { get; set; } // Navigeringsfält för transaktionen som orsakar relationen
        public int TargetTransactionId { get; set; } // Transaktionen som påverkas av relationen (t.ex. ursprungliga köpet)
        public Transaction TargetTransaction { get; set; } // Navigeringsfält för transaktionen som påverkas
    }
}