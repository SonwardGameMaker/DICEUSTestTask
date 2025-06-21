using Microsoft.Extensions.Options;
using Telegram.Bot;
using Telegram.Bot.Types;
using TelegramBotDiseusTestApp.DTOs.Options;
using TelegramBotDiseusTestApp.Services;

namespace TelegramBotDiseusTestApp.AiChat
{
    internal class AiChatManager
    {
        private int _maxNumberOfChats;
        private ITelegramBotClient _bot;
        private TelegramDataTransferService _telegramService;
        private MindeeService _mindeeService;
        private GroqService _groqService;
        private List<AiChat> _chats;

        public AiChatManager(ITelegramBotClient bot, TelegramDataTransferService telegramService, MindeeService mindeeService, GroqService groqService, IOptions<AiChatManagerOptions> options)
        {
            _maxNumberOfChats = options.Value.MaxNumberOfChats;
            _bot = bot;
            _telegramService = telegramService;
            _mindeeService = mindeeService;
            _groqService = groqService;
            _chats = new List<AiChat>();
        }

        public async Task TalkToChat(Update update)
        {
            var message = update.Message;
            if (message == null)
                return;

            var chat = _chats.Find(c => c.Chat.Id == message.Chat.Id);
            if (chat == null)
                chat = CreateChat(message.Chat);
            if (chat == null) return;

            if (message.Text == "/resetchat")
                chat.ResetChat();
            else
                await chat.TalkToChat(message);
        }


        private AiChat? CreateChat(Chat chat)
        {
            if (_chats.Count < _maxNumberOfChats)
            {
                var newChat = new AiChat(chat, _bot, _telegramService, _mindeeService, _groqService);
                newChat.JobDone += OnJobDone;

                _chats.Add(newChat);
                return newChat;
            }
            return null;
        }

        private void OnJobDone(long id)
        {
            var toDelete = _chats.Find(c => c.Chat.Id == id);

            if (toDelete == null) return;
            toDelete.JobDone -= OnJobDone;

            _chats.Remove(toDelete);
        }
    }
}
