using System.Globalization;
using MediatR;
using Microsoft.Data.Sqlite;
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

            public Handler(
                PiggyzenContext context,
                TransactionService transactionService,
                CategoryService categoryService)
            {
                _context = context;
                _transactionService = transactionService;
                _categoryService = categoryService;
            }

            public async Task<Result> Handle(Command request, CancellationToken cancellationToken)
            {
                if (string.IsNullOrWhiteSpace(request.TransactionData))
                {
                    throw new ArgumentException("Transaction data cannot be empty.");
                }

                var lines = request.TransactionData.Split('\n', StringSplitOptions.RemoveEmptyEntries);
                var transactions = new List<Transaction>();

                foreach (var line in lines)
                {
                    var columns = line.Split('\t');
                    if (columns.Length < 5)
                    {
                        continue;
                    }

                    try
                    {
                        var bookingDate = TryParseDate(columns[0]);
                        var transactionDate = TryParseDate(columns[1]);
                        var description = columns[2].Trim();
                        var amount = TryParseDecimal(columns[3]);
                        var balance = TryParseDecimal(columns[4]);

                        var referenceId = _transactionService.GenerateReferenceId(
                            bookingDate,
                            transactionDate ?? DateTime.MinValue,
                            description,
                            amount,
                            balance
                        );

                        var exists = await _context.Transactions
                            .AnyAsync(t => t.ReferenceId == referenceId, cancellationToken);

                        if (exists)
                        {
                            continue;
                        }

                        // Försök föreslå en kategori
                        var suggestedCategoryId = await _categoryService.SuggestCategoryIdAsync(description);

                        var transaction = new Transaction
                        {
                            BookingDate = bookingDate,
                            TransactionDate = transactionDate ?? DateTime.MinValue,
                            Description = description,
                            Amount = amount,
                            Balance = balance,
                            ReferenceId = referenceId,
                            CategoryId = suggestedCategoryId,
                            CategorizationStatus = suggestedCategoryId.HasValue
                                ? CategorizationStatusEnum.AutoCategorized
                                : CategorizationStatusEnum.NotCategorized
                        };

                        transactions.Add(transaction);
                    }
                    catch (FormatException)
                    {
                        throw new FormatException($"Error in data format on line: {line}");
                    }
                    catch
                    {
                        throw;
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

            private DateTime? TryParseDate(string dateInput)
            {
                if (DateTime.TryParse(dateInput, out DateTime parsedDate))
                {
                    return parsedDate;
                }

                return null;
            }

            private decimal TryParseDecimal(string decimalInput)
            {
                if (decimal.TryParse(decimalInput.Replace(",", "."), NumberStyles.Any, CultureInfo.InvariantCulture, out decimal parsedDecimal))
                {
                    return parsedDecimal;
                }

                return 0;
            }
        }
    }
}
/* using System.Globalization;
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

            public Handler(
                PiggyzenContext context,
                TransactionService transactionService,
                CategoryService categoryService)
            {
                _context = context;
                _transactionService = transactionService;
                _categoryService = categoryService;
            }

            public async Task<Result> Handle(Command request, CancellationToken cancellationToken)
            {
                if (string.IsNullOrWhiteSpace(request.TransactionData))
                {
                    throw new ArgumentException("Transaction data cannot be empty.");
                }

                var lines = request.TransactionData.Split('\n', StringSplitOptions.RemoveEmptyEntries);
                var transactions = new List<Transaction>();

                foreach (var line in lines)
                {
                    var columns = line.Split('\t');
                    if (columns.Length < 5)
                    {
                        continue;
                    }

                    try
                    {
                        var bookingDate = TryParseDate(columns[0]);
                        var transactionDate = TryParseDate(columns[1]);
                        var description = columns[2].Trim();
                        var amount = TryParseDecimal(columns[3]);
                        var balance = TryParseDecimal(columns[4]);

                        var referenceId = _transactionService.GenerateReferenceId(
                            bookingDate,
                            transactionDate ?? DateTime.MinValue,
                            description,
                            amount,
                            balance
                        );

                        var exists = await _context.Transactions
                            .AnyAsync(t => t.ReferenceId == referenceId, cancellationToken);

                        if (exists)
                        {
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

                        transactions.Add(transaction);
                    }
                    catch (FormatException)
                    {
                        throw new FormatException($"Error in data format on line: {line}");
                    }
                    catch
                    {
                        throw;
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

            private DateTime? TryParseDate(string dateInput)
            {
                if (DateTime.TryParse(dateInput, out DateTime parsedDate))
                {
                    return parsedDate;
                }

                return null;
            }

            private decimal TryParseDecimal(string decimalInput)
            {
                if (decimal.TryParse(decimalInput.Replace(",", "."), NumberStyles.Any, CultureInfo.InvariantCulture, out decimal parsedDecimal))
                {
                    return parsedDecimal;
                }

                return 0;
            }
        }
    }
} */