namespace piggyzen.api.Models
{
    public class Transaction
    {
        public int Id { get; set; }
        public DateOnly BookingDate { get; set; }
        public DateOnly TransactionDate { get; set; }
        public string Description { get; set; }
        public decimal Amount { get; set; }
        public decimal Balance { get; set; }
        public string BookTransDescAmountBalanceID { get; set; }
        public int CategoryId { get; set; }
        public Category Category { get; set; }
    }
}