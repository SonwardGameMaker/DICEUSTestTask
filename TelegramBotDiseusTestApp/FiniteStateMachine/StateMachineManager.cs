using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using TelegramBotDiseusTestApp.Services;

namespace TelegramBotDiseusTestApp.FiniteStateMachine
{
    internal class StateMachineManager
    {
        private int _maxNumberOfChats;
        private TelegramBotClient _bot;
        private TelegramDataTransferService _telegramService;
        private MindeeService _mindeeService;
        private OpenAiService _openAiService;
        private List<ChatStateMachine> _chats;

        private bool _inited;

        public StateMachineManager(int maxNumberOfStateMachines)
        {
            _inited = false;
            _chats = new List<ChatStateMachine>();
            _maxNumberOfChats = maxNumberOfStateMachines;
        }

        public void Init(TelegramBotClient bot, TelegramDataTransferService telegramService, MindeeService mindeeService, OpenAiService openAiService)
        {
            if (_inited) return;

            _bot = bot;
            _telegramService = telegramService;
            _mindeeService = mindeeService;
            _openAiService = openAiService;

            _inited = true;
        }

        public int MaxNumberOfChats { get => _maxNumberOfChats; }

        public async Task Execute(Message message, UpdateType updateType)
        {
            if (_chats.Find(c => c.Chat == message.Chat) == null)
            {
                AddStateMachine(message.Chat);
            }
            await _chats.Find(c => c.Chat == message.Chat).Execute(message);
        }
        public async Task Execute(Update update)
        {
            if (_chats.Find(c => c.Chat == update.Message.Chat) == null)
            {
                AddStateMachine(update.Message.Chat);
            }
            await _chats.Find(c => c.Chat == update.Message.Chat).Execute(update);
        }

        private bool AddStateMachine(Chat chat)
        {
            if (_chats.Count < _maxNumberOfChats)
            {
                _chats.Add(new ChatStateMachine(chat, _bot, _telegramService, _mindeeService, _openAiService));
                return true;
            }
            return false;
        }
    }
}
