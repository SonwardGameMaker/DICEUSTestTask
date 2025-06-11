using Mindee.Product.DriverLicense;
using Mindee.Product.Passport;
using Telegram.Bot;
using Telegram.Bot.Types;
using TelegramBotDiseusTestApp.FiniteStateMachine.ChatStates;
using TelegramBotDiseusTestApp.FiniteStateMachine.ChatStates.Concrete;
using TelegramBotDiseusTestApp.Services;

namespace TelegramBotDiseusTestApp.FiniteStateMachine
{
    internal class ChatStateMachine
    {
        private readonly Chat _chat;
        private readonly TelegramBotClient _bot;
        private readonly TelegramDataTransferService _telegramService;
        private readonly List<ChatState> _states;
        private ChatState _currentState;

        public event Action? JobDone;

        public ChatStateMachine(Chat chat, TelegramBotClient bot, TelegramDataTransferService telegramService, MindeeService mindeeService, OpenAiService openAiService)
        {
            _chat = chat;
            _bot = bot;
            _telegramService = telegramService;

            PassportPhotoPath = "";
            DriverLicensePhotoPath = "";
            Passport = new PassportV1();
            DriverLicense = new DriverLicenseV1();

            _states = InitStates(mindeeService, openAiService);
            GetState<InsurancePolicyIssuanceState>().JobDone += ClearPhotoData;

            _currentState = _states[0];
            _currentState.EnterState();
        }

        public Chat Chat { get =>  _chat; }
        public TelegramBotClient Bot { get => _bot; }
        public TelegramDataTransferService TelegramService { get => _telegramService; }

        public string PassportPhotoPath {  get; set; }
        public string DriverLicensePhotoPath { get; set; }
        public  PassportV1 Passport { get; set; }
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

        private List<ChatState> InitStates(MindeeService mindeeService, OpenAiService openAiService)
        {
            List<ChatState> chatStates = new List<ChatState>();

            chatStates.Add(new PassportRequirementState(this));
            chatStates.Add(new DriverLicenseRequirementState(this, mindeeService));
            chatStates.Add(new PriceQuotationState(this));
            chatStates.Add(new InsurancePolicyIssuanceState(this, openAiService));

            return chatStates;
        }

        private void ClearPhotoData()
        {
            GetState<DriverLicenseRequirementState>().ClearData();
            GetState<InsurancePolicyIssuanceState>().JobDone -= ClearPhotoData;

            JobDone?.Invoke();
        }
    }
}
