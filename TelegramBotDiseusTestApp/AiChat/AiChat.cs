using GroqNet.ChatCompletions;
using Mindee.Product.DriverLicense;
using Mindee.Product.InternationalId;
using System;
using Telegram.Bot;
using Telegram.Bot.Types;
using TelegramBotDiseusTestApp.DTOs;
using TelegramBotDiseusTestApp.Services;

namespace TelegramBotDiseusTestApp.AiChat
{
    internal partial class AiChat
    {
        private readonly ITelegramBotClient _bot;
        private readonly TelegramDataTransferService _telegramService;
        private readonly MindeeService _mindeeService;
        private readonly GroqService _groqService;

        private readonly Chat _chat;
        private GroqChatHistory _chatHistory;
        private UserCurrentData _userCurrentData;
        private string _passportPhotoPath;
        private string _driverLicensePhotoPath;
        private InternationalIdV2 _passport;
        private DriverLicenseV1 _driverLicense;

        public event Action? JobDone;

        public AiChat(Chat chat, ITelegramBotClient bot, TelegramDataTransferService telegramService, MindeeService mindeeService, GroqService groqService)
        {
            _chat = chat;
            _bot = bot;
            _telegramService = telegramService;
            _mindeeService = mindeeService;
            _groqService = groqService;

            ResetChat();

            _groqService.ToMuchTokenUse += ToMuchTokensInform;
        }
        ~AiChat()
        {
            _groqService.ToMuchTokenUse -= ToMuchTokensInform;
        }

        public Chat Chat { get => _chat; }

        public async Task TalkToChat(Message message)
        {
            string promt = $"{(await GetDocumentPhoto(message)? "[User added photo]; " : "")} + {(message.Text != null ? message.Text : "")}";

            if (promt.Length > 0)
                await HandleGroqRespond(_groqService.AskAsync(promt, _userCurrentData, _chatHistory));
            else
                await _bot.SendMessage(_chat.Id, "Please write some text or send photo");
        }

        public void ResetChat()
        {
            _chatHistory = _groqService.CreateChatHistory();
            _userCurrentData = new UserCurrentData(_chat.FirstName);

            _passportPhotoPath = "";
            _driverLicensePhotoPath = "";
            _passport = new InternationalIdV2();
            _driverLicense = new DriverLicenseV1();
        }

        private async Task HandleGroqRespond(Task<string> respond)
        {
            string input = await respond;
            string text;
            string command;

            int commandStart = input.LastIndexOf('[');
            int commandEnd = input.LastIndexOf(']');

            text = input.Substring(0, commandStart).Trim();
            command = input.Substring(commandStart + 1, commandEnd - commandStart - 1).Trim();

            await _bot.SendMessage(_chat.Id, text);

            if (command != null && command.Length > 0)
                await ExecuteAiCommand(command);
        }

        private async Task<bool> GetDocumentPhoto(Message message)
        {
            if (message == null || message.Photo == null) return false;

            if (_passportPhotoPath == "")
            {
                _passportPhotoPath = await _telegramService.GetPhoto(message, DocumentType.Passport);
                _userCurrentData.PassportPhotoUploaded = true;
                return true;
            }
            else if (_driverLicensePhotoPath == "")
            {
                _driverLicensePhotoPath = await _telegramService.GetPhoto(message, DocumentType.DriverLicense);
                _userCurrentData.DriverLicensePhotoUploaded = true;
                return true;
            }
            return false;
        }

        private async void ToMuchTokensInform(string message)
            => await _bot.SendMessage(_chat, message);
    }
}
