using GroqNet.ChatCompletions;
using Mindee.Product.DriverLicense;
using Mindee.Product.InternationalId;
using Telegram.Bot;
using Telegram.Bot.Types;
using TelegramBotDiseusTestApp.DTOs;
using TelegramBotDiseusTestApp.FiniteStateMachine.ChatStates;
using TelegramBotDiseusTestApp.FiniteStateMachine.ChatStates.Concrete;
using TelegramBotDiseusTestApp.Services;

namespace TelegramBotDiseusTestApp.FiniteStateMachine
{
    internal class ChatStateMachine
    {
        private readonly ITelegramBotClient _bot;
        private readonly TelegramDataTransferService _telegramService;
        private readonly GroqService _groqService;

        private readonly Chat _chat;
        private readonly List<ChatState> _states;
        private ChatState _currentState;
        private GroqChatHistory _groqChatHistory;

        public event Action<long>? JobDone;

        public ChatStateMachine(Chat chat, ITelegramBotClient bot, TelegramDataTransferService telegramService, MindeeService mindeeService, GroqService groqService)
        {
            _chat = chat;
            _bot = bot;
            _telegramService = telegramService;
            _groqService = groqService;

            UserCurrentData = new UserCurrentData(chat.FirstName);
            _groqChatHistory = _groqService.CreateChatHistory();

            PassportPhotoPath = "";
            DriverLicensePhotoPath = "";
            Passport = new InternationalIdV2();
            DriverLicense = new DriverLicenseV1();

            _states = InitStates(mindeeService);
            GetState<InsurancePolicyIssuanceState>().JobDone += ClearPhotoData;

            _currentState = _states[0];
            _currentState.EnterState();
        }
        ~ChatStateMachine()
        {
            GetState<InsurancePolicyIssuanceState>().JobDone -= ClearPhotoData;
        }

        public Chat Chat { get =>  _chat; }
        public ITelegramBotClient Bot { get => _bot; }
        public TelegramDataTransferService TelegramService { get => _telegramService; }
        public GroqService GroqService { get => _groqService; }

        public UserCurrentData UserCurrentData;
        public GroqChatHistory ChatHistory { get => _groqChatHistory; }
        public string PassportPhotoPath {  get; set; }
        public string DriverLicensePhotoPath { get; set; }
        public  InternationalIdV2 Passport { get; set; }
        public DriverLicenseV1 DriverLicense { get; set; }

        public void ChangeSate<T>() where T : ChatState
        {
            _currentState.ExitState();
            _currentState = _states.Find(s => s is T) as T;
            _currentState.EnterState();
        }

        public T GetState<T>() where T : ChatState
            => _states.Find(s => s is T) as T;

        public async Task Execute(Message message)
            => await _currentState.Execute(message);
        public async Task Execute(Update update)
            => await _currentState.Execute(update);

        private List<ChatState> InitStates(MindeeService mindeeService)
        {
            List<ChatState> chatStates = new List<ChatState>();

            chatStates.Add(new GreetingsState(this));
            chatStates.Add(new PassportRequirementState(this));
            chatStates.Add(new DriverLicenseRequirementState(this, mindeeService));
            chatStates.Add(new PriceQuotationState(this));
            chatStates.Add(new InsurancePolicyIssuanceState(this));

            return chatStates;
        }

        private void ClearPhotoData()
        {
            GetState<DriverLicenseRequirementState>().ClearData();
            GetState<InsurancePolicyIssuanceState>().JobDone -= ClearPhotoData;

            UserCurrentData.PhotosConfirmed = false;
            UserCurrentData.PriceConfirmed = true;

            JobDone?.Invoke(_chat.Id);
        }
    }
}
