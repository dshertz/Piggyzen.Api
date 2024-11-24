using System.Globalization;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Piggyzen.Api.Data;
using Piggyzen.Api.Services;

namespace Piggyzen.Api.Features.Transactions
{
    public class ImportFromText
    {
        public class Command : IRequest<Result>
        {
            public string TransactionData { get; set; }
        }

        public class Result
        {
            public string Message { get; set; }
            public int Count { get; set; }
        }

        public class Handler : IRequestHandler<Command, Result>
        {
            private readonly PiggyzenContext _context;
            private readonly TransactionService _transactionService;
            private readonly CategoryService _categoryService;

            public Handler(PiggyzenContext context, TransactionService transactionService, CategoryService categoryService)
            {
                _context = context;
                _transactionService = transactionService;
                _categoryService = categoryService;
            }

            public async Task<Result> Handle(Command request, CancellationToken cancellationToken)
            {
                if (string.IsNullOrWhiteSpace(request.TransactionData))
                    throw new ArgumentException("Transaction data cannot be empty.");

                var lines = request.TransactionData.Split('\n', StringSplitOptions.RemoveEmptyEntries);
                var transactions = new List<Transaction>();

                foreach (var line in lines)
                {
                    var columns = line.Split('\t');
                    if (columns.Length < 5) continue;

                    try
                    {
                        var bookingDate = TryParseDate(columns[0]);
                        var transactionDate = TryParseDate(columns[1]);
                        var description = columns[2].Trim();
                        var amount = TryParseDecimal(columns[3]);
                        var balance = TryParseDecimal(columns[4]);

                        // Generera ReferenceId
                        var referenceId = _transactionService.GenerateReferenceId(
                            bookingDate,
                            transactionDate ?? DateTime.MinValue,
                            description,
                            amount,
                            balance
                        );

                        // Kontrollera om transaktionen redan finns
                        var exists = await _context.Transactions
                            .AnyAsync(t => t.ReferenceId == referenceId, cancellationToken);

                        if (exists)
                        {
                            // Hoppa över denna rad om den redan finns
                            continue;
                        }

                        var transaction = new Transaction
                        {
                            BookingDate = bookingDate,
                            TransactionDate = transactionDate ?? DateTime.MinValue,
                            Description = description,
                            Amount = amount,
                            Balance = balance,
                            ReferenceId = referenceId,
                        };

                        // Tilldela kategori
                        await _categoryService.AssignOrSuggestCategoryAsync(transaction, null);

                        transactions.Add(transaction);
                    }
                    catch (FormatException)
                    {
                        throw new FormatException($"Error in data format on line: {line}");
                    }
                }

                if (transactions.Any())
                {
                    _context.Transactions.AddRange(transactions);
                    await _context.SaveChangesAsync(cancellationToken);

                    return new Result
                    {
                        Message = "Transactions imported successfully.",
                        Count = transactions.Count
                    };
                }

                return new Result
                {
                    Message = "No valid transactions to import.",
                    Count = 0
                };
            }

            // Metod för att tolka och rensa datumformat
            private DateTime? TryParseDate(string dateInput)
            {
                return DateTime.TryParse(dateInput, out DateTime parsedDate) ? parsedDate : (DateTime?)null;
            }

            // Metod för att tolka och rensa decimalformat
            private decimal TryParseDecimal(string decimalInput)
            {
                return decimal.TryParse(decimalInput.Replace(",", "."), NumberStyles.Any, CultureInfo.InvariantCulture, out decimal parsedDecimal)
                    ? parsedDecimal
                    : 0;
            }
        }
    }
}