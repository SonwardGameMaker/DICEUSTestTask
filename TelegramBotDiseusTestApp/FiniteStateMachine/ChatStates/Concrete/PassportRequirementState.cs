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
            await _stateMachine.Bot.SendMessage(_stateMachine.Chat, _response.PassportRequest);
        }

        public override async Task Execute(Message message)
        {
            if (message.Photo != null)
            {
                _stateMachine.PassportPhotoPath = await _stateMachine.TelegramService.GetPhoto(message, DocumentType.Passport);
                _stateMachine.ChangeSate<DriverLicenseRequirementState>();
            }
            else
            {
                if (message.Text != null)
                    await AskGroq(message.Text);
                else
                    await _stateMachine.Bot.SendMessage(_stateMachine.Chat, _response.DidntSendPassportResponse);
            }
                
        }

        public override async Task Execute(Update update)
            => await _stateMachine.Bot.SendMessage(_stateMachine.Chat, _response.DidntSendPassportResponse);

        public override void ExitState() { }
    }
}
