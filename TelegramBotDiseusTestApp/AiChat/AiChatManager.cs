using Telegram.Bot;
using Telegram.Bot.Types;
using TelegramBotDiseusTestApp.FiniteStateMachine;
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

        public AiChatManager(int maxNumberOfChats, ITelegramBotClient bot, TelegramDataTransferService telegramService, MindeeService mindeeService, GroqService groqService)
        {
            _maxNumberOfChats = maxNumberOfChats;
            _bot = bot;
            _telegramService = telegramService;
            _mindeeService = mindeeService;
            _groqService = groqService;
            _chats = new List<AiChat>();
        }

        public async Task TalkToChat(Message message)
        {
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

        private void OnJobDone()
        {
            // TODO
        }
    }
}
