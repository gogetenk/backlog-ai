using AgileMind.Application.Domain;
using AgileMind.Application.Interfaces;
using Azure.AI.OpenAI;
using Microsoft.Extensions.Logging;

namespace AgileMind.Infrastructure.Gpt;

public class GptClient : IAiClient
{
    private readonly ILogger<GptClient> _logger;
    private readonly OpenAIClient _openAIClient;
    private readonly JsonProcessor _jsonProcessor;

    public GptClient(ILogger<GptClient> logger, OpenAIClient openAIClient, JsonProcessor jsonProcessor)
    {
        _logger = logger;
        _openAIClient = openAIClient;
        _jsonProcessor = jsonProcessor;
    }

    public async Task<Backlog> GenerateBacklogFromPrompt(string userPrompt, CancellationToken cancellationToken)
    {
        var prompt = BuildPrompt(userPrompt);
        var chatCompletionsOptions = new ChatCompletionsOptions()
        {
            //DeploymentName = "gpt-4-1106-preview",
            DeploymentName = "gpt-3.5-turbo-1106",
            Messages =
                {
                    new ChatRequestSystemMessage(prompt)
                }
        };
        var streamingResponse = await _openAIClient.GetChatCompletionsStreamingAsync(chatCompletionsOptions, cancellationToken);

        var backlog = new Backlog(string.Empty, string.Empty);
        await foreach (var update in streamingResponse)
        {
            var responseMessage = update.ContentUpdate;
            if (!string.IsNullOrWhiteSpace(responseMessage))
            {
                _jsonProcessor.ProcessStreamedText(responseMessage, backlog);
            }
        }

        return backlog;
    }

    private string BuildPrompt(string userPrompt)
    {
        return $"As an Agile Expert and Scrum Master, transform the following requirement into a structured backlog with user stories : '{userPrompt}'" +
               "Each user story should have a title, description, and a complexity estimation following the Fibonacci sequence. " +
               "Please provide a RFC8259 compliant JSON response without any explanations, following this format:\n" +
               "{\n" +
               "  \"backlogTitle\": \"<Title Here (should be short)>\",\n" +
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
}
