namespace piggyzen.api.Dtos.Transaction
{
    public class UpdateTransactionDto
    {
        public int Id { get; set; }
        public DateTime BookingDate { get; set; }
        public DateTime TransactionDate { get; set; }
        public string? Description { get; set; }
        public decimal Amount { get; set; }
        public decimal Balance { get; set; }
        public int? CategoryId { get; set; }
        public string? ImportId { get; set; }
    }
}