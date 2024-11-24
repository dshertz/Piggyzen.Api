using MediatR;
using Microsoft.AspNetCore.Mvc;
using Piggyzen.Api.Features.Tags;

namespace Piggyzen.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TagController : ControllerBase
    {
        private readonly IMediator _mediator;

        public TagController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllTags()
        {
            var result = await _mediator.Send(new GetAllTags.Query());
            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetTagById(int id)
        {
            var query = new GetTagById.Query { Id = id };
            var result = await _mediator.Send(query);
            return result == null ? NotFound($"Tag with ID {id} not found.") : Ok(result);
        }

        [HttpPost]
        public async Task<IActionResult> CreateTag(CreateTag.Command command)
        {
            var result = await _mediator.Send(command);
            return CreatedAtAction(nameof(GetTagById), new { id = result.Id }, result);
        }


        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateTag(int id, UpdateTag.Command command)
        {
            command.Id = id;
            await _mediator.Send(command);
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTag(int id)
        {
            var command = new DeleteTag.Command { Id = id };
            await _mediator.Send(command);
            return NoContent();
        }
    }
}