using MediatR;
using Microsoft.AspNetCore.Mvc;
using Piggyzen.Api.Features.Transactions;

namespace Piggyzen.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TransactionController : ControllerBase
    {
        private readonly IMediator _mediator;

        public TransactionController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllTransactions()
        {
            var result = await _mediator.Send(new GetAllTransactions.Query());
            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetTransactionById(int id)
        {
            var result = await _mediator.Send(new GetTransactionById.Query { Id = id });
            return result == null ? NotFound() : Ok(result);
        }

        [HttpPost("create-complete")]
        public async Task<IActionResult> CreateCompleteTransaction(CreateCompleteTransaction.Command command)
        {
            var result = await _mediator.Send(command);
            return CreatedAtAction(nameof(GetTransactionById), new { id = result.Id }, result);
        }

        [HttpPost("create-partial")]
        public async Task<IActionResult> CreatePartialTransaction(CreatePartialTransaction.Command command)
        {
            var result = await _mediator.Send(command);
            return CreatedAtAction(nameof(GetTransactionById), new { id = result.Id }, result);
        }

        [HttpPost("import-text")]
        [Consumes("text/plain")]
        public async Task<IActionResult> ImportFromText([FromBody] string transactionData)
        {
            var result = await _mediator.Send(new ImportFromText.Command { TransactionData = transactionData });
            return result.Count > 0 ? Ok(result) : BadRequest(result.Message);
        }

        [HttpPut("complete-update/{id}")]
        public async Task<IActionResult> CompleteUpdateTransaction(int id, UpdateCompleteTransaction.Command command)
        {
            command.Id = id;
            await _mediator.Send(command);
            return NoContent();
        }

        [HttpPut("partial-update/{id}")]
        public async Task<IActionResult> PartialUpdateTransaction(int id, UpdatePartialTransaction.Command command)
        {
            command.Id = id;
            var result = await _mediator.Send(command);
            return Ok(result);
        }

        /*  [HttpPut("{transactionId}/category/{categoryId}")]
         public async Task<IActionResult> AssignCategory(int transactionId, int categoryId)
         {
             Console.WriteLine($"API hit: transactionId={transactionId}, categoryId={categoryId}");
             var command = new AssignCategory.Command
             {
                 TransactionId = transactionId,
                 CategoryId = categoryId
             };
             await _mediator.Send(command);
             return NoContent();
         } */
        [HttpPut("assign-category")]
        public async Task<IActionResult> AssignCategory([FromBody] AssignCategory.Command command)
        {
            if (command.TransactionIds == null || !command.TransactionIds.Any())
            {
                Console.WriteLine("No transactions provided in the request.");
                return BadRequest("No transactions provided.");
            }

            Console.WriteLine($"API hit: Assigning category {command.CategoryId} to {command.TransactionIds.Count} transactions.");

            await _mediator.Send(command);

            return NoContent();
        }

        [HttpPost("approve-category")]
        public async Task<IActionResult> ApproveCategory([FromBody] List<int> transactionIds)
        {
            if (transactionIds == null || !transactionIds.Any())
            {
                return BadRequest("No transactions selected for approval.");
            }

            await _mediator.Send(new ApproveCategory.Command { TransactionIds = transactionIds });
            return Ok("Categories approved successfully!");
        }

        [HttpDelete]
        [HttpPost("delete")]
        public async Task<IActionResult> DeleteTransactions([FromBody] List<int> transactionIds)
        {
            if (transactionIds == null || !transactionIds.Any())
            {
                return BadRequest("No transaction IDs provided.");
            }

            var command = new DeleteTransaction.Command { TransactionIds = transactionIds };
            await _mediator.Send(command);

            return NoContent();
        }

        /* [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTransaction(int id)
        {
            var command = new DeleteTransaction.Command { Id = id };
            await _mediator.Send(command);
            return NoContent();
        } */

        [HttpPost("categorize")]
        public async Task<IActionResult> CategorizeUncategorized()
        {
            var result = await _mediator.Send(new CategorizeUncategorizedTransactions.Command());
            return Ok(result);
        }

        [HttpGet("{transactionId}/similar")]
        public async Task<IActionResult> GetSimilarTransactions(int transactionId)
        {
            var similarTransactions = await _mediator.Send(new GetSimilarTransactions.Query { TransactionId = transactionId });
            return Ok(similarTransactions);
        }

        [HttpPost("get-selected-transactions")]
        public async Task<IActionResult> GetSelectedTransactions([FromBody] List<int> transactionIds)
        {
            var result = await _mediator.Send(new GetSelectedTransactions.Query { TransactionIds = transactionIds });
            return Ok(result);
        }
    }
}