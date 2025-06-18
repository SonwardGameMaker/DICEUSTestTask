using GroqNet;
using GroqNet.ChatCompletions;
using TelegramBotDiseusTestApp.DTOs;

namespace TelegramBotDiseusTestApp.Services
{
    internal class GroqService
    {
        private readonly GroqClient _ai;
        private BotInstructions _instructions;

        public GroqService(string apiKey, GroqModel model, BotInstructions? botInstructions = null)
        {
            _ai = new GroqClient(apiKey, model);
            if (botInstructions == null)
            {
                _instructions = BotInstructions.DefaultInstructions;
            }
            else
                _instructions = botInstructions;
        }

        public BotInstructions Instructions { get => _instructions; }

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

        public async Task<string> AskAsync(string prompt, UserCurrentData userData, GroqChatHistory chatHistory)
        {
            try
            {
                chatHistory.Add(new GroqMessage(GroqChatRole.System, UserDataToPromt(userData)));
                chatHistory.AddUserMessage(prompt);
                var rsp = await _ai.GetChatCompletionsAsync(chatHistory);
                var result = rsp.Choices.First().Message.Content;
                chatHistory.AddAssistantMessage(result);
                return result;
            }
            catch (Exception ex)
            {
                Console.WriteLine("ERROR: " + ex.Message);
                return "ERROR: " + ex.Message;
            }
        }

        public GroqChatHistory CreateChatHistory()
            => new GroqChatHistory { _instructions.BaseInstructions };

        private string UserDataToPromt(UserCurrentData userData)
            => $"User name: {userData.UserName}" +
            $"\nPassport photo uploaded: {userData.PassportPhotoUploaded}" +
            $"\nDriver license photo uploaded: {userData.DriverLicensePhotoUploaded}" +
            $"\nPhotos confirmed: {userData.PhotosConfirmed}" +
            $"\nPrice confirmed: {userData.PriceConfirmed}";
    }
}
