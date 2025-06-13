using Telegram.Bot.Types;
using TelegramBotDiseusTestApp.DTOs;

namespace TelegramBotDiseusTestApp.FiniteStateMachine.ChatStates
{
    internal abstract class ChatState
    {
        protected ChatStateMachine _stateMachine;
        protected BotResponseData _response;

        public ChatState(ChatStateMachine chatStateMachine)
        {
            _stateMachine = chatStateMachine;
            _response = BotResponseData.TempResponceData;
        }

        public abstract void ExitState();
        public abstract void EnterState();

        public abstract Task Execute(Message message);
        public abstract Task Execute(Update update);
    }
}
