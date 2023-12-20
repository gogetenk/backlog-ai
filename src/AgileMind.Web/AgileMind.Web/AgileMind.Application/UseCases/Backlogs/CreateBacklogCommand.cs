using AgileMind.Application.Domain;
using AgileMind.Application.Interfaces;
using MediatR;

namespace AgileMind.Application.UseCases.Backlogs;

public record CreateBacklogCommand(string UserPrompt) : IRequest<BacklogDto>
{
    public class CreateBacklogCommandHandler : IRequestHandler<CreateBacklogCommand, BacklogDto>
    {
        private readonly IBacklogRepository _repository;
        private readonly IAiClient _aiClient;

        public CreateBacklogCommandHandler(IBacklogRepository repository, IAiClient aiClient)
        {
            _repository = repository;
            _aiClient = aiClient;
        }

        public async Task<BacklogDto> Handle(CreateBacklogCommand request, CancellationToken cancellationToken)
        {
            // Using GPT service to generate backlog details
            var generatedBacklog = await _aiClient.GenerateBacklogFromPrompt(request.UserPrompt);
            var backlog = new Backlog(generatedBacklog.Title, generatedBacklog.Description);

            foreach (var story in generatedBacklog.UserStories)
            {
                backlog.AddUserStory(new UserStory(story.Title, story.Description));
            }

            await _repository.AddAsync(backlog);

            return new BacklogDto
            {
                Id = backlog.Id,
                Title = backlog.Title,
                Description = backlog.Description,
                UserStories = backlog.UserStories.Select(story => new UserStoryDto
                {
                    Id = story.Id,
                    Title = story.Title,
                    Description = story.Description,
                    IsCompleted = story.IsCompleted
                }).ToList()
            };
        }
    }
}