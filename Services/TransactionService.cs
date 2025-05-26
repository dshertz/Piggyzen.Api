using Microsoft.AspNetCore.SignalR;
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

        public async Task UpdateHasSimilarFlagsAsync(CancellationToken cancellationToken)
        {
            // Kontrollera om det finns AutoCategorized-transaktioner
            var autoCategorizedTransactions = await _context.Transactions
                .Where(t => t.CategorizationStatus == CategorizationStatusEnum.AutoCategorized)
                .ToListAsync(cancellationToken);

            if (!autoCategorizedTransactions.Any())
            {
                // Avbryt om inga AutoCategorized-transaktioner finns
                return;
            }

            // Skapa en lista för att samla uppdateringar
            var transactionsToUpdate = new List<Transaction>();

            // Kontrollera för liknande transaktioner
            foreach (var transaction in autoCategorizedTransactions)
            {
                // Sök efter liknande transaktioner i ett enda query
                var hasSimilar = await _context.Transactions
                    .Where(t =>
                        t.Description == transaction.Description &&
                        t.Id != transaction.Id &&
                        t.CategorizationStatus == CategorizationStatusEnum.AutoCategorized)
                    .AnyAsync(cancellationToken);

                if (transaction.HasSimilar != hasSimilar)
                {
                    transaction.HasSimilar = hasSimilar;
                    transactionsToUpdate.Add(transaction);
                }
            }

            // Spara ändringarna endast om något faktiskt ändrats
            if (transactionsToUpdate.Any())
            {
                await _context.SaveChangesAsync(cancellationToken);
            }
        }


        // Hitta liknande transaktioner baserat på beskrivning och status, med möjlighet att exkludera en viss transaktion
        public async Task<bool> HasSimilarTransactionsAsync(string description, int excludeTransactionId, CategorizationStatusEnum currentStatus, CancellationToken cancellationToken)
        {
            // Kontrollera om transaktionen är ManuallyCategorized och returnera false direkt
            if (currentStatus == CategorizationStatusEnum.ManuallyCategorized)
            {
                return false;
            }

            // Kontrollera liknande transaktioner endast för AutoCategorized eller andra tillåtna statusar
            return await _context.Transactions
                .Where(t =>
                    t.Description == description &&
                    t.Id != excludeTransactionId &&
                    t.CategorizationStatus == CategorizationStatusEnum.AutoCategorized) // Endast AutoCategorized
                .AnyAsync(cancellationToken);
        }

        public async Task<List<Transaction>> FindSimilarTransactionsAsync(string description, int excludeTransactionId, CancellationToken cancellationToken)
        {
            return await _context.Transactions
                .Where(t =>
                    t.Description == description &&
                    t.Id != excludeTransactionId &&
                    t.CategorizationStatus == CategorizationStatusEnum.AutoCategorized) // Endast AutoCategorized
                .ToListAsync(cancellationToken);
        }

        /* // Kontrollerar om det finns autokategoriserade transaktioner som har liknande beskrivningar.
        // Uppdaterar HasSimilar-flaggan om det behövs och meddelar eventuella ändringar till klienter via SignalR.
        public async Task CheckAndNotifySimilarTransactionsAsync(CancellationToken cancellationToken)
        {
            // Hämta alla AutoCategorized transaktioner
            var autoCategorizedTransactions = await _context.Transactions
                .Where(t => t.CategorizationStatus == CategorizationStatusEnum.AutoCategorized)
                .ToListAsync(cancellationToken);

            // Console.WriteLine($"Found {autoCategorizedTransactions.Count} AutoCategorized transactions.");

            if (!autoCategorizedTransactions.Any())
            {
                // Avbryt om inga AutoCategorized-transaktioner finns
                return;
            }

            var transactionsToUpdate = new List<Transaction>();
            var transactionsToNotify = new List<Transaction>();

            // Kontrollera och gruppera liknande transaktioner
            foreach (var transaction in autoCategorizedTransactions)
            {
                // Hitta liknande transaktioner direkt i databasen
                var hasSimilar = await _context.Transactions
                    .Where(t =>
                        t.Description == transaction.Description &&
                        t.Id != transaction.Id &&
                        t.CategorizationStatus == CategorizationStatusEnum.AutoCategorized)
                    .AnyAsync(cancellationToken);

                if (transaction.HasSimilar != hasSimilar)
                {
                    // Uppdatera HasSimilar-flaggan
                    transaction.HasSimilar = hasSimilar;
                    transactionsToUpdate.Add(transaction);

                    // Lägg till i listan för SignalR-uppdatering
                    transactionsToNotify.Add(transaction);
                }
            }

            // Spara alla ändringar på en gång
            if (transactionsToUpdate.Any())
            {
                await _context.SaveChangesAsync(cancellationToken);
            }
        } */

    }
}