using Microsoft.EntityFrameworkCore;
using Piggyzen.Api.Data;

namespace Piggyzen.Api.Services
{
    public class TransactionService
    {
        private readonly PiggyzenContext _context;

        public TransactionService(PiggyzenContext context)
        {
            _context = context;
        }

        // Genererar ett unikt ReferenceId baserat på transaktionsfält
        public string GenerateReferenceId(DateTime? bookingDate, DateTime transactionDate, string description, decimal amount, decimal? balance)
        {
            // Hanterar nullvärden med standardtext eller belopp
            string bookingDatePart = bookingDate.HasValue ? bookingDate.Value.ToString("yyyyMMdd") : "N/A";
            string balancePart = balance.HasValue ? balance.Value.ToString("F2") : "N/A";

            // Genererar ReferenceId
            return $"{bookingDatePart}|{transactionDate:yyyyMMdd}|{description}|{amount:F2}|{balancePart}";
        }
    }
}