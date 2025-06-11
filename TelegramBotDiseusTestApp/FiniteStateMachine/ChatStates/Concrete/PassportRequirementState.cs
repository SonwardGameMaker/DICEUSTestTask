using Telegram.Bot;
using Telegram.Bot.Types;

namespace TelegramBotDiseusTestApp.FiniteStateMachine.ChatStates.Concrete
{
    internal class PassportRequirementState : ChatState
    {
        public PassportRequirementState(ChatStateMachine chatStateMachine) : base(chatStateMachine)
        {
        }

        public override async void EnterState()
        {
            await _stateMachine.Bot.SendMessage(_stateMachine.Chat, BotResponseData.TempResponceData.GreetingsResponse);
            await _stateMachine.Bot.SendMessage(_stateMachine.Chat, BotResponseData.TempResponceData.PassportRequest);
        }

        public override async Task Execute(Message message)
        {
            _stateMachine.PassportPhotoPath = await _stateMachine.TelegramService.GetPhoto(message, DocumentType.Passport);
            _stateMachine.ChangeSate<DriverLicenseRequirementState>();
        }

        public override async Task Execute(Update update)
            => await _stateMachine.Bot.SendMessage(_stateMachine.Chat, BotResponseData.TempResponceData.DidntSendPassportResponse);

        public override void ExitState() { }
    }
}
