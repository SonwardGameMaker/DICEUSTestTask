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
                _instructions = BotInstructions.DefaultInstructions;
            else
                _instructions = botInstructions;
        }

        public async Task<string> TalkToChat(string prompt)
        {
            var history = new GroqChatHistory { new(prompt) };
            return await ExecuteChatCompletionAsync(history);
        }

        public async Task<string> TalkToChat(string prompt, UserCurrentData userData, GroqChatHistory chatHistory)
        {
            chatHistory.Add(new GroqMessage(GroqChatRole.System, UserDataToPromt(userData)));
            chatHistory.AddUserMessage(prompt);
            ValidateTokenNumber(chatHistory);
            return await ExecuteChatCompletionAsync(chatHistory);
        }

        public GroqChatHistory CreateChatHistory()
            => new GroqChatHistory { _instructions.BaseInstructions, _instructions.CommandList, _instructions.Rules };



        private async Task<string> ExecuteChatCompletionAsync(GroqChatHistory chatHistory)
        {
            try
            {
                var respond = await _groqCient.GetChatCompletionsAsync(chatHistory);
                var result = respond.Choices.First().Message.Content;
                chatHistory.AddAssistantMessage(result);
                return result;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return ex.Message;
            }
        }

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
                chatHistory.Remove(chatHistory.First(c => c.Role != GroqChatRole.System));
        }

        private int AppoximateTokenNumber(GroqChatHistory chatHistory)
        {
            int result = 0;

            foreach(GroqMessage message in chatHistory)
                result += (int)Math.Ceiling(message.Content.Split(' ').Length * 3.0 / 4.0);

            return result;
        }
    }
}
