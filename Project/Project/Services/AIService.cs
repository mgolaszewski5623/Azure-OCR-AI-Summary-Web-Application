using Azure;
using Azure.AI.OpenAI;
using OpenAI.Chat;

namespace Project.Services
{
    public class AIService
    {
        private readonly ChatClient _chatClient;

        public AIService(IConfiguration config)
        {
            var client = new AzureOpenAIClient(
                new Uri(config["AzureOpenAI:Endpoint"]),
                new AzureKeyCredential(config["AzureOpenAI:ApiKey"])
            );

            _chatClient = client.GetChatClient(
                config["AzureOpenAI:DeploymentName"]
            );
        }

        public async Task<string> SummarizeAsync(string text)
        {
            var requestOptions = new ChatCompletionOptions()
            {
                MaxOutputTokenCount = 16384,
                Temperature = 1.0f,
                TopP = 1.0f,

            };
            List<ChatMessage> messages = new List<ChatMessage>()
            {
                new SystemChatMessage("You are a helpful assistant."),
                new UserChatMessage(text),
            };

            var response = _chatClient.CompleteChat(messages, requestOptions);
            return response.Value.Content[0].Text;
        }
    }
}
