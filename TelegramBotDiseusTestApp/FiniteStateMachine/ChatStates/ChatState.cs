using Telegram.Bot.Types;

namespace TelegramBotDiseusTestApp.FiniteStateMachine.ChatStates
{
    internal abstract class ChatState
    {
        protected ChatStateMachine _stateMachine;

        public ChatState(ChatStateMachine chatStateMachine)
        {
            _stateMachine = chatStateMachine;
        }

        public abstract void ExitState();
        public abstract void EnterState();

        public abstract Task Execute(Message message);
        public abstract Task Execute(Update update);
    }
}
