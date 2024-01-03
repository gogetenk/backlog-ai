using AgileMind.Application.Domain;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;
using System.Text;
using System.Text.Json;

namespace AgileMind.Infrastructure.Gpt;

public class JsonProcessor
{
    private readonly ILogger<JsonProcessor> _logger;
    private readonly IHubContext<BacklogHub> _backlogHubContext;
    private StringBuilder _stringBuilder = new();

    public JsonProcessor(ILogger<JsonProcessor> logger, IHubContext<BacklogHub> backlogHubContext)
    {
        _logger = logger;
        _backlogHubContext = backlogHubContext;
    }

    public void ProcessStreamedText(string streamedText, Backlog backlog)
    {
        _logger.LogInformation($"Processing text: {streamedText}");
        _stringBuilder.Append(streamedText);

        TryProcessUserStories(backlog);
    }

    private void TryProcessUserStories(Backlog backlog)
    {
        int arrayStart = _stringBuilder.ToString().IndexOf('[');
        if (arrayStart == -1)
            return; // Attendez d'atteindre le début du tableau des User Stories

        int storyStart, storyEnd;
        while ((storyStart = _stringBuilder.ToString().IndexOf('{', arrayStart)) != -1 &&
               (storyEnd = _stringBuilder.ToString().IndexOf('}', storyStart)) != -1)
        {
            string userStoryJson = _stringBuilder.ToString(storyStart, storyEnd - storyStart + 1);
            if (IsValidUserStoryJson(userStoryJson))
            {
                ProcessJsonObject(userStoryJson, backlog);
                _stringBuilder.Remove(storyStart, storyEnd - storyStart + 1);
            }
            else
            {
                break; // JSON incomplet, attendre plus de données
            }
        }
    }

    private bool IsValidUserStoryJson(string json)
    {
        return json.Contains("\"title\":") && json.Contains("\"description\":") && json.Contains("\"complexity\":");
    }

    private async Task ProcessJsonObject(string json, Backlog backlog)
    {
        try
        {
            if (json.Contains("\"title\":") && json.Contains("\"description\":"))
            {
                UserStoryData storyData = JsonSerializer.Deserialize<UserStoryData>(json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true })!;
                if (storyData != null)
                {
                    _logger.LogInformation($"Adding a User Story: {storyData.Title}");
                    var newUserStory = new UserStory(storyData.Title, storyData.Description, storyData.Complexity);
                    backlog.AddUserStory(newUserStory);
                    await _backlogHubContext.Clients.All.SendAsync("ReceiveUserStory", newUserStory);
                }
            }
            else
            {
                BacklogData backlogData = JsonSerializer.Deserialize<BacklogData>(json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true })!;
                if (backlogData != null)
                {
                    _logger.LogInformation($"Adding backlog data: {backlogData.BacklogTitle}");
                    backlog.UpdateTitle(backlogData.BacklogTitle);
                    backlog.UpdateDescription(backlogData.BacklogDescription);
                }
            }
        }
        catch (JsonException ex)
        {
            _logger.LogError($"JSON parsing error: {ex.Message}");
            // Error handling logic for incomplete or invalid JSON
        }
    }
}
