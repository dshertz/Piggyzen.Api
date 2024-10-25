using System.Globalization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using piggyzen.api.Data;
using piggyzen.api.Dtos.Transaction;

namespace piggyzen.api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TransactionController : ControllerBase

    {
        private readonly PiggyzenContext _context;
        public TransactionController(PiggyzenContext context)
        {
            _context = context;
        }
        [HttpGet]
        public async Task<ActionResult<List<GetAllTransactionsDto>>> GetAllTransactions()
        {
            var transactions = await _context.Transactions.Include(c => c.Category).ThenInclude(t => t.ParentCategory).Select(c => new
            {
                Id = c.Id,
                BookingDate = c.BookingDate.ToString("yyyy-MM-dd"),
                Description = c.Description.Trim().ToLower().Normalize(),
                Amount = c.Amount.ToString("F2", CultureInfo.InvariantCulture),
            }).ToListAsync();
            return Ok(transactions);
        }
        [HttpGet("{id}")]
        public async Task<ActionResult<GetTransactionByIdDto>> GetTransactionById(int id)
        {
            var transaction = await _context.Transactions
            .Include(t => t.Category)
            .ThenInclude(c => c.ParentCategory)
            .Where(t => t.Id == id)
            .Select(t => new
            {
                Id = t.Id,
                BookingDate = t.BookingDate,
                TransactionDate = t.TransactionDate,
                Description = t.Description,
                Amount = t.Amount,
                Balance = t.Balance,
                CategoryName = t.Category.Name,
                ParentCategoryName = t.Category.ParentCategory != null
                ? t.Category.ParentCategory.Name
                : null
            })
        .SingleOrDefaultAsync();

            if (transaction is null)
            {
                return NotFound();
            }
            return Ok(transaction);
        }

        [HttpPost]
        public async Task<ActionResult<CreateTransactionDto>> CreateTransaction(CreateTransactionDto transactionDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (!string.IsNullOrEmpty(transactionDto.ImportId))
            {
                var existingTransaction = await _context.Transactions.SingleOrDefaultAsync(t => t.ImportId == transactionDto.ImportId);

                if (existingTransaction != null)
                {
                    return Conflict("En transaktion med detta ImportId finns redan.");
                }
            }

            var transaction = new Transaction
            {
                BookingDate = transactionDto.BookingDate,
                TransactionDate = transactionDto.TransactionDate,
                Description = transactionDto.Description.Trim(),
                Amount = transactionDto.Amount,
                Balance = transactionDto.Balance,
                CategoryId = transactionDto.CategoryId,
                ImportId = transactionDto.ImportId
            };

            _context.Transactions.Add(transaction);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetTransactionById), new { id = transaction.Id }, transaction);
        }
        [HttpPut("{id}")]
        public async Task<ActionResult> UpdateTransaction(int id, UpdateTransactionDto model)
        {
            if (!ModelState.IsValid) return BadRequest("Information saknas för att kunna uppdatera transaktionen");

            // Kontrollera om transaktionen finns
            var transaction = await _context.Transactions.FindAsync(id);
            if (transaction == null) return NotFound($"Vi kan inte hitta en transaktion med id: {id}");

            // Uppdatera transaktionens egenskaper
            transaction.BookingDate = model.BookingDate;
            transaction.TransactionDate = model.TransactionDate;
            transaction.Description = model.Description?.Trim();
            transaction.Amount = model.Amount;
            transaction.Balance = model.Balance;
            transaction.CategoryId = model.CategoryId;
            transaction.ImportId = model.ImportId;

            _context.Transactions.Update(transaction);

            if (await _context.SaveChangesAsync() > 0)
            {
                return NoContent();
            }

            return StatusCode(500, "Internal Server Error");
        }
        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteTransaction(int id)
        {
            var transaction = await _context.Transactions.FindAsync(id);

            if (transaction == null)
            {
                return NotFound($"Transaktionen med id {id} hittades inte.");
            }

            _context.Transactions.Remove(transaction);

            if (await _context.SaveChangesAsync() > 0)
            {
                return NoContent();
            }

            return StatusCode(500, "Ett fel inträffade vid borttagning av transaktionen.");
        }

    }
}