using Telegram.Bot;
using Telegram.Bot.Types;
using TelegramBotDiseusTestApp.Services;

namespace TelegramBotDiseusTestApp.FiniteStateMachine
{
    internal class StateMachineManager
    {
        private int _maxNumberOfChats;
        private ITelegramBotClient _bot;
        private TelegramDataTransferService _telegramService;
        private MindeeService _mindeeService;
        private GroqService _groqService;
        private List<ChatStateMachine> _chats;

        private bool _inited;

        public StateMachineManager(int maxNumberOfStateMachines)
        {
            _inited = false;
            _chats = new List<ChatStateMachine>();
            _maxNumberOfChats = maxNumberOfStateMachines;
        }

        public void Init(ITelegramBotClient bot, TelegramDataTransferService telegramService, MindeeService mindeeService, GroqService groqService)
        {
            if (_inited) return;

            _bot = bot;
            _telegramService = telegramService;
            _mindeeService = mindeeService;
            _groqService = groqService;

            _inited = true;
        }

        public int MaxNumberOfChats { get => _maxNumberOfChats; }

        public async Task Execute(Update update)
        {
            if (update.Message is { } message)
            {
                if (_chats.Find(c => c.Chat.Id == message.Chat.Id) == null)
                {
                    AddStateMachine(message.Chat);
                    Console.WriteLine($"State machine added via message. Chat: {message.Chat}");
                }

                await _chats.Find(c => c.Chat.Id == message.Chat.Id).Execute(message);
                return;
            }

            if (update.CallbackQuery is { } query && query.Message is { } callbackMessage)
            {
                if (_chats.Find(c => c.Chat.Id == callbackMessage.Chat.Id) == null)
                {
                    AddStateMachine(callbackMessage.Chat);
                    Console.WriteLine($"State machine added via callback. Chat: {callbackMessage.Chat}");
                }

                await _chats.Find(c => c.Chat.Id == callbackMessage.Chat.Id).Execute(update);
            }
        }

        private bool AddStateMachine(Chat chat)
        {
            if (_chats.Count < _maxNumberOfChats)
            {
                var newStateMachine = new ChatStateMachine(chat, _bot, _telegramService, _mindeeService, _groqService);
                newStateMachine.JobDone += OnJobDone;
                
                _chats.Add(newStateMachine);
                return true;
            }
            return false;
        }

        private void OnJobDone(long chatId)
        {
            var toDelete = _chats.Find(c => c.Chat.Id ==  chatId);

            if (toDelete == null) return;
            toDelete.JobDone -= OnJobDone;

            _chats.Remove(toDelete);
        }
    }
}
