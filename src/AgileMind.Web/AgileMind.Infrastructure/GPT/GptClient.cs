﻿using AgileMind.Application.Domain;
using AgileMind.Application.Interfaces;
using Azure;
using Azure.AI.OpenAI;
using System.Text.Json;

namespace AgileMind.Infrastructure.GPT
{
    public class GptClient : IAiClient
    {
        private readonly string azureOpenAIResourceUri = "https://my-resource.openai.azure.com/";
        private readonly AzureKeyCredential azureOpenAIApiKey = new(Environment.GetEnvironmentVariable("AZURE_OPENAI_API_KEY"));

        public async Task<Backlog> GenerateBacklogFromPrompt(string userPrompt)
        {
            var client = new OpenAIClient(new Uri(azureOpenAIResourceUri), azureOpenAIApiKey);
            var prompt = BuildPrompt(userPrompt);

            var chatCompletionsOptions = new ChatCompletionsOptions()
            {
                DeploymentName = "gpt-3.5-turbo",
                Messages =
                {
                    new ChatRequestSystemMessage(prompt)
                }
            };

            Response<ChatCompletions> response = await client.GetChatCompletionsAsync(chatCompletionsOptions);
            ChatResponseMessage responseMessage = response.Value.Choices[0].Message;

            Backlog generatedBacklog = ProcessGeneratedText(responseMessage.Content);

            return generatedBacklog;
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
                   "Important: don't write anything else than the required json. No introduction. No politeness. NOTHING.";
        }


        private Backlog ProcessGeneratedText(string generatedText)
        {
            try
            {
                var options = new JsonSerializerOptions
                {
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                    WriteIndented = true
                };

                var backlogData = JsonSerializer.Deserialize<BacklogData>(generatedText, options);
                if (backlogData == null)
                {
                    throw new InvalidOperationException("Unable to parse the GPT response into a backlog.");
                }

                var backlog = new Backlog(backlogData.BacklogTitle, backlogData.BacklogDescription);
                foreach (var story in backlogData.UserStories)
                {
                    backlog.AddUserStory(new UserStory(story.Title, story.Description, story.));
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
