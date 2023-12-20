using AgileMind.Application.Domain;
using AgileMind.Application.Interfaces;
using Azure;
using Azure.AI.OpenAI;

namespace AgileMind.Infrastructure.GPT
{
    public class GptClient : IAiClient
    {
        private readonly string apiKey = "YOUR_API_KEY";

        public async Task<Backlog> GenerateBacklogFromPrompt(string userPrompt)
        {
            var client = new OpenAIClient("your-api-key-from-platform.openai.com");
            var chatCompletionsOptions = new ChatCompletionsOptions()
            {
                DeploymentName = "gpt-4-0613", // Use DeploymentName for "model" with non-Azure clients
                Messages =
                {
                    // The system message represents instructions or other guidance about how the assistant should behave
                    new ChatRequestSystemMessage("You are a helpful assistant. You will talk like a pirate."),
                    // User messages represent current or historical input from the end user
                    new ChatRequestUserMessage("Can you help me?"),
                    // Assistant messages represent historical responses from the assistant
                    new ChatRequestAssistantMessage("Arrrr! Of course, me hearty! What can I do for ye?"),
                    new ChatRequestUserMessage("What's the best way to train a parrot?"),
                }
            };

            Response<ChatCompletions> response = await client.GetChatCompletionsAsync(chatCompletionsOptions);
            ChatResponseMessage responseMessage = response.Value.Choices[0].Message;

            // Process the generated text and create a Backlog object
            //Backlog generatedBacklog = ProcessGeneratedText(responseMessage.Content);

            return new("Toto","titi");
        }

        //private Backlog ProcessGeneratedText(string generatedText)
        //{
        //    // Split the generated text into individual lines
        //    string[] lines = generatedText.Split('\n');

        //    // Create a new Backlog object
        //    Backlog backlog = new Backlog();

        //    // Iterate through each line of the generated text
        //    foreach (string line in lines)
        //    {
        //        // Trim any leading or trailing whitespace
        //        string trimmedLine = line.Trim();

        //        // Skip empty lines
        //        if (string.IsNullOrEmpty(trimmedLine))
        //        {
        //            continue;
        //        }

        //        // Create a new Task object
        //        Task task = new Task();

        //        // Set the task description to the trimmed line
        //        task.Description = trimmedLine;

        //        // Add the task to the backlog
        //        backlog.Tasks.Add(task);
        //    }

        //    return backlog;
        //}
    }
}
