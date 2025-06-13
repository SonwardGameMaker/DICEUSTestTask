using GroqNet;
using GroqNet.ChatCompletions;

namespace TelegramBotDiseusTestApp.Services
{
    internal class GroqService
    {
        private readonly GroqClient _ai;

        public GroqService(string apiKey, GroqModel model)
        {
            _ai = new GroqClient(apiKey, model);
        }

        public async Task<string> AskAsync(string prompt)
        {
            try
            {
                var history = new GroqChatHistory { new(prompt) };
                var rsp = await _ai.GetChatCompletionsAsync(history);
                return rsp.Choices.First().Message.Content;
            }
            catch (Exception ex)
            {
                Console.WriteLine("ERROR: " + ex.Message);
                return "ERROR: " + ex.Message;
            }
        }
    }
}
