using GroqNet;
using GroqNet.ChatCompletions;
using TelegramBotDiseusTestApp.DTOs;

namespace TelegramBotDiseusTestApp.Services
{
    internal class GroqService
    {
        private readonly GroqModel _model;
        private readonly GroqClient _groqCient;
        private BotInstructions _instructions;

        public Action<string> ToMuchTokenUse;

        public GroqService(string apiKey, GroqModel model, BotInstructions? botInstructions = null)
        {
            _model = model;
            _groqCient = new GroqClient(apiKey, model);
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
                var rsp = await _groqCient.GetChatCompletionsAsync(history);
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
                ValidateTokenNumber(chatHistory);
                var rsp = await _groqCient.GetChatCompletionsAsync(chatHistory);
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

        public async Task<string> AskAsSystenAsync(string prompt, UserCurrentData userData, GroqChatHistory chatHistory)
        {
            try
            {
                chatHistory.Add(new GroqMessage(GroqChatRole.System, UserDataToPromt(userData)));
                chatHistory.AddUserMessage(prompt);
                ValidateTokenNumber(chatHistory);
                var rsp = await _groqCient.GetChatCompletionsAsync(chatHistory);
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
            => new GroqChatHistory { _instructions.BaseInstructions, _instructions.CommandList, _instructions.Rules };

        private string UserDataToPromt(UserCurrentData userData)
            => $"User name: {userData.UserName}" +
            $"\nPassport photo uploaded: {userData.PassportPhotoUploaded}" +
            $"\nDriver license photo uploaded: {userData.DriverLicensePhotoUploaded}" +
            $"\nPhotos confirmed: {userData.PhotosConfirmed}" +
            $"\nPrice confirmed: {userData.PriceConfirmed}";

        private void ValidateTokenNumber(GroqChatHistory chatHistory)
        {
            if (AppoximateTokenNumber(chatHistory) <= GroqModel.MaxTokens(_model)) return;

            ToMuchTokenUse?.Invoke(BotResponseData.DefaultResponceData.ToMuchTokenUseWarning);
            while (AppoximateTokenNumber(chatHistory) > GroqModel.MaxTokens(_model))
            {
                chatHistory.Remove(chatHistory.First(c => c.Role != GroqChatRole.System));
            }
        }

        private int AppoximateTokenNumber(GroqChatHistory chatHistory)
        {
            int result = 0;

            foreach(GroqMessage message in chatHistory)
            {
                result += (int)Math.Ceiling(message.Content.Split(' ').Length * 3.0 / 4.0);
            }

            return result;
        }
    }
}
