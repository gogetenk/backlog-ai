using AgileMind.Application.Domain;
using AgileMind.Application.Interfaces;
using Azure.AI.OpenAI;
using Microsoft.Extensions.Logging;
using System.Text;
using System.Text.Json;

namespace AgileMind.Infrastructure.Gpt
{
    public class GptClient : IAiClient
    {
        private readonly string azureOpenAIApiKey = "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";
        private readonly ILogger<GptClient> _logger;

        public GptClient(ILogger<GptClient> logger)
        {
            _logger = logger;
        }

        public async Task<Backlog> GenerateBacklogFromPrompt(string userPrompt, CancellationToken cancellationToken)
        {
            var client = new OpenAIClient(azureOpenAIApiKey);
            var prompt = BuildPrompt(userPrompt);

            var chatCompletionsOptions = new ChatCompletionsOptions()
            {
                DeploymentName = "gpt-3.5-turbo-1106",
                Messages =
                {
                    new ChatRequestSystemMessage(prompt)
                }
            };

            var streamingResponse = await client.GetChatCompletionsStreamingAsync(chatCompletionsOptions, cancellationToken);

            var backlog = new Backlog(string.Empty, string.Empty);
            await foreach (var update in streamingResponse)
            {
                var responseMessage = update.ContentUpdate;

                if (!string.IsNullOrWhiteSpace(responseMessage))
                {
                    ProcessStreamedText(responseMessage, backlog);
                }
            }

            return backlog;
        }

        StringBuilder stringBuilder = new();
        bool isInUserStoriesArray = false;

        private void ProcessStreamedText(string streamedText, Backlog backlog)
        {
            _logger.LogInformation($"Processing text {streamedText}");
            stringBuilder.Append(streamedText);

            string currentJson = stringBuilder.ToString();
            ProcessBacklogAndUserStories(currentJson, backlog);
        }

        private void ProcessBacklogAndUserStories(string json, Backlog backlog)
        {
            int lastProcessedIndex = 0;
            int openBraces = 0;

            for (int i = 0; i < json.Length; i++)
            {
                if (json[i] == '[')
                {
                    _logger.LogInformation($"-- Start user stories");
                    isInUserStoriesArray = true;
                }
                else if (json[i] == ']')
                {
                    _logger.LogInformation($"-- End user stories");
                    isInUserStoriesArray = false;
                }

                if (isInUserStoriesArray)
                {
                    if (json[i] == '{')
                    {
                        if (openBraces == 0)
                        {
                            _logger.LogInformation($"---- Début d'une UserStory");
                            lastProcessedIndex = i; // Début d'une UserStory
                        }
                        openBraces++;
                    }
                    else if (json[i] == '}')
                    {
                        openBraces--;
                        if (openBraces == 0) // Fin d'une UserStory
                        {
                            _logger.LogInformation($"---- Fin d'une UserStory");
                            string objectJson = json.Substring(lastProcessedIndex, i - lastProcessedIndex + 1);
                            _logger.LogInformation($"------------------------------- Adding an US");
                            ProcessJsonObject(objectJson, backlog);
                        }
                    }
                }
            }

            // Nettoyer le stringBuilder si nécessaire
            if (lastProcessedIndex > 0 && !isInUserStoriesArray && openBraces == 0)
            {
                stringBuilder.Remove(0, lastProcessedIndex);
            }
        }

        private void ProcessJsonObject(string json, Backlog backlog)
        {
            try
            {
                if (json.Contains("\"title\":") && json.Contains("\"description\":"))
                {
                    // C'est une UserStory
                    UserStoryData storyData = JsonSerializer.Deserialize<UserStoryData>(json, new JsonSerializerOptions() { PropertyNameCaseInsensitive = true });
                    if (storyData != null)
                    {
                        _logger.LogInformation($"------------------------------- Adding an US : {storyData.Title}");
                        backlog.AddUserStory(new UserStory(storyData.Title, storyData.Description, storyData.Complexity));
                    }
                }
                else
                {
                    // C'est peut-être le début du backlog
                    BacklogData backlogData = JsonSerializer.Deserialize<BacklogData>(json, new JsonSerializerOptions() { PropertyNameCaseInsensitive = true });
                    if (backlogData != null)
                    {
                        _logger.LogInformation($"Adding a backlog : {backlogData.BacklogTitle}");
                        backlog.UpdateTitle(backlogData.BacklogTitle);
                        backlog.UpdateDescription(backlogData.BacklogDescription);
                    }
                }
            }
            catch (JsonException ex)
            {
                _logger.LogError($"JSON parsing error: {ex.Message}");
                // Vous pouvez choisir de ne rien faire ici, car cela peut être dû à un JSON incomplet
            }
        }


        private void UpdateBacklog(Backlog backlog, BacklogData backlogData)
        {
            if (string.IsNullOrEmpty(backlog.Title))
            {
                backlog.UpdateTitle(backlogData.BacklogTitle);
                backlog.UpdateDescription(backlogData.BacklogDescription);
            }
        }


        // Méthode pour vérifier si une chaîne représente un objet JSON complet
        private bool IsCompleteJson(string json)
        {
            try
            {
                JsonDocument.Parse(json);
                return true;
            }
            catch (JsonException)
            {
                return false;
            }
        }
        private string RemoveMarkdown(string text)
        {
            return text.Replace("```json", "", StringComparison.OrdinalIgnoreCase)
                       .Replace("```", "", StringComparison.OrdinalIgnoreCase);
        }

        private string BuildPrompt(string userPrompt)
        {
            return $"As an Agile Expert and Scrum Master, transform the following requirement into a structured backlog with user stories : '{userPrompt}'" +
                   "Each user story should have a title, description, and a complexity estimation following the Fibonacci sequence. " +
                   "Please provide a RFC8259 compliant JSON response without any explanations, following this format:\n" +
                   "{\n" +
                   "  \"backlogTitle\": \"<Title Here>\",\n" +
                   "  \"backlogDescription\": \"<Description Here>\",\n" +
                   "  \"userStories\": [\n" +
                   "    {\n" +
                   "      \"title\": \"<User Story Title>\",\n" +
                   "      \"description\": \"<User Story Description>\",\n" +
                   "      \"complexity\": <Fibonacci Number>\n" +
                   "    }\n" +
                   "    // ...\n" +
                   "  ]\n" +
                   "}\n" +
                   "Important: don't write anything else than the required json. No introduction. No politeness. No markdown code to wrap the json. NOTHING.";
        }

        private Backlog ProcessGeneratedText(string generatedText)
        {
            try
            {
                // Remove Markdown code block syntax (triple backticks)
                string cleanedGeneratedText = generatedText
                    .Replace("```Json", "", StringComparison.OrdinalIgnoreCase)
                    .Replace("```", "", StringComparison.OrdinalIgnoreCase);

                var options = new JsonSerializerOptions
                {
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                    WriteIndented = true
                };

                var backlogData = JsonSerializer.Deserialize<BacklogData>(cleanedGeneratedText, options);
                if (backlogData == null)
                {
                    throw new InvalidOperationException("Unable to parse the GPT response into a backlog.");
                }

                var backlog = new Backlog(backlogData.BacklogTitle, backlogData.BacklogDescription);
                foreach (var story in backlogData.UserStories)
                {
                    backlog.AddUserStory(new UserStory(story.Title, story.Description, story.Complexity));
                }

                return backlog;
            }
            catch (JsonException ex)
            {
                // Handle JSON parsing error
                throw new InvalidOperationException("JSON parsing error: " + ex.Message, ex);
            }
        }

        private class BacklogData
        {
            public string BacklogTitle { get; set; }
            public string BacklogDescription { get; set; }
            public List<UserStoryData> UserStories { get; set; } = new List<UserStoryData>();
        }

        private class UserStoryData
        {
            public string Title { get; set; }
            public string Description { get; set; }
            public int Complexity { get; set; } // Added property
        }

    }
}
