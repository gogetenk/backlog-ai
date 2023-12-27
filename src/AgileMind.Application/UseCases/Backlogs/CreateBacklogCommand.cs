using AgileMind.Application.Domain;
using AgileMind.Application.Interfaces;
using MediatR;

namespace AgileMind.Application.UseCases.Backlogs;

public record CreateBacklogCommand(string UserPrompt) : IRequest<Backlog>
{
    public class CreateBacklogCommandHandler : IRequestHandler<CreateBacklogCommand, Backlog>
    {
        private readonly IBacklogRepository _repository;
        private readonly IAiClient _aiClient;

        public CreateBacklogCommandHandler(IBacklogRepository repository, IAiClient aiClient)
        {
            _repository = repository;
            _aiClient = aiClient;
        }

        public async Task<Backlog> Handle(CreateBacklogCommand request, CancellationToken cancellationToken)
        {
            // Using GPT service to generate backlog details
            var generatedBacklog = await _aiClient.GenerateBacklogFromPrompt(request.UserPrompt, cancellationToken);
            var backlog = new Backlog(generatedBacklog.Title, generatedBacklog.Description);

            foreach (var story in generatedBacklog.UserStories)
            {
                backlog.AddUserStory(new UserStory(story.Title, story.Description, story.Complexity));
            }

            //await _repository.AddAsync(backlog);

            return backlog;
        }
    }
}