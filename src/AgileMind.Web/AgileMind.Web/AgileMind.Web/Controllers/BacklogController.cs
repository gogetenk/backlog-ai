using AgileMind.Application.UseCases.Backlogs;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace AgileMind.Web.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class BacklogController : ControllerBase
    {
        private readonly ISender _sender;

        public BacklogController(ISender sender)
        {
            _sender = sender;
        }

        //[HttpGet]
        //public async Task<IActionResult> GetAllBacklogs()
        //{
        //    var query = new GetAllBacklogsQuery();
        //    var result = await _sender.Send(query);
        //    return Ok(result);
        //}

        //[HttpGet("{id}")]
        //public async Task<IActionResult> GetBacklogById(string id)
        //{
        //    var query = new GetBacklogByIdQuery(id);
        //    var result = await _sender.Send(query);
        //    return result != null ? Ok(result) : NotFound();
        //}

        [HttpPost]
        public async Task<IActionResult> CreateBacklog(CreateBacklogCommand command)
        {
            var result = await _sender.Send(command);
            return CreatedAtAction("GetBacklogById", new { id = result.Id }, result);
        }

        //[HttpPut("{id}")]
        //public async Task<IActionResult> UpdateBacklog(string id, UpdateBacklogCommand command)
        //{
        //    if (id != command.Id)
        //    {
        //        return BadRequest();
        //    }

        //    var result = await _sender.Send(command);
        //    return NoContent();
        //}

        // Add other endpoints as needed
    }
}
