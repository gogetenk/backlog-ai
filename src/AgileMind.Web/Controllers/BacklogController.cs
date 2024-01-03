using AgileMind.Application.UseCases.Backlogs;
using AgileMind.Web.Dto;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace AgileMind.Web.Controllers;

[ApiController]
[Route("api/[controller]")]
public class BacklogController : ControllerBase
{
    private readonly ISender _sender;

    public BacklogController(ISender sender)
    {
        _sender = sender;
    }

    [HttpPost]
    public async Task<IActionResult> CreateBacklog(CreateBacklogCommand command)
    {
        var result = await _sender.Send(command);
        var dto = new BacklogDto
        {
            Id = result.Id,
            Title = result.Title,
            Description = result.Description,
            UserStories = result.UserStories.Select(story => new UserStoryDto
            {
                Id = story.Id,
                Title = story.Title,
                Description = story.Description,
                IsCompleted = story.IsCompleted,

            }).ToList()
        };
        return CreatedAtAction("GetBacklogById", new { id = dto.Id }, dto);
    }
}
