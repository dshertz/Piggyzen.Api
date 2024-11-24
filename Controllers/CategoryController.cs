using MediatR;
using Microsoft.AspNetCore.Mvc;
using Piggyzen.Api.Features.Categories;

namespace Piggyzen.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CategoryController : ControllerBase
    {
        private readonly IMediator _mediator;

        public CategoryController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllCategories()
        {
            var result = await _mediator.Send(new GetAllCategories.Query());
            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetCategoryById(int id)
        {
            var query = new GetCategoryById.Query { Id = id };
            var result = await _mediator.Send(query);
            return result == null ? NotFound($"Category with ID {id} not found.") : Ok(result);
        }

        [HttpPost]
        public async Task<IActionResult> CreateCategory(CreateCategory.Command command)
        {
            var result = await _mediator.Send(command);
            return CreatedAtAction(nameof(GetCategoryById), new { id = result.Id }, result);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateCategory(int id, UpdateCategory.Command command)
        {
            command.Id = id;
            await _mediator.Send(command);
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCategory(int id)
        {
            var command = new DeleteCategory.Command { Id = id };
            await _mediator.Send(command);
            return NoContent();
        }

        [HttpGet("hierarchy")]
        public async Task<IActionResult> GetCategoryHierarchy()
        {
            var result = await _mediator.Send(new GetCategoryHierarchy.Query());
            return Ok(result);
        }
    }
}