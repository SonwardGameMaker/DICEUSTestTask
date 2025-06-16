using Telegram.Bot;
using Telegram.Bot.Types;

namespace TelegramBotDiseusTestApp.FiniteStateMachine.ChatStates.Concrete
{
    internal class GreetingsState : ChatState
    {
        public GreetingsState(ChatStateMachine chatStateMachine) : base(chatStateMachine) { }

        public override async void EnterState() { }

        public override async Task Execute(Message message) 
        {
            if (message.Text == _response.StartCommand)
            {
                await _stateMachine.Bot.SendMessage(_stateMachine.Chat, _response.GreetingsResponse);
                _stateMachine.ChangeSate<PassportRequirementState>();
            }
        }

        public override async Task Execute(Update update) { }

        public override void ExitState() { }
    }
}
