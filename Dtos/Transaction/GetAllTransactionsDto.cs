namespace piggyzen.api.Dtos.Transaction
{
    public class GetAllTransactionsDto
    {
        public DateTime BookingDate { get; set; }
        public string Description { get; set; }
        public decimal Amount { get; set; }
        public int? CategoryId { get; set; }
        public Category? Category { get; set; }
    }
}