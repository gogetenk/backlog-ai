using AgileMind.Application.Domain;
using AgileMind.Application.Interfaces;

namespace AgileMind.Infrastructure.Gpt;

public class FakeAiClient : IAiClient
{
    public Task<Backlog> GenerateBacklogFromPrompt(string userPrompt, CancellationToken cancellationToken)
    {
        // Création d'un backlog factice avec titre et description
        var backlog = new Backlog($"Backlog pour: {userPrompt}", "Description factice du backlog");

        // Ajout de User Stories factices au backlog
        backlog.AddUserStory(CreateFakeUserStory("User Story 1", "Description de User Story 1", 3));
        backlog.AddUserStory(CreateFakeUserStory("User Story 2", "Description de User Story 2", 2));
        backlog.AddUserStory(CreateFakeUserStory("User Story 3", "Description de User Story 3", 2));
        backlog.AddUserStory(CreateFakeUserStory("User Story 4", "Description de User Story 4", 2));
        backlog.AddUserStory(CreateFakeUserStory("User Story 5", "Description de User Story 5", 2));
        // Ajoutez plus de User Stories si nécessaire

        return Task.FromResult(backlog);
    }

    private UserStory CreateFakeUserStory(string title, string description, int complexity)
    {
        // Création d'une User Story factice
        return new UserStory(title, description, complexity);
    }
}
