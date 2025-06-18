using Telegram.Bot;
using Telegram.Bot.Types;
using TelegramBotDiseusTestApp.Services;

namespace TelegramBotDiseusTestApp.FiniteStateMachine.ChatStates.Concrete
{
    internal class InsurancePolicyIssuanceState : ChatState
    {
        public event Action? JobDone;

        public InsurancePolicyIssuanceState(ChatStateMachine chatStateMachine) : base(chatStateMachine) { }

        public override async void EnterState()
        {
            string chatGptResponce = await _stateMachine.GroqService.AskAsync($"Generate a dummy insurance policy based on data:\n" +
                $"{_stateMachine.Passport.Prediction.ToString()}\n" +
                $"{_stateMachine.DriverLicense.Prediction.ToString()}");
            await _stateMachine.Bot.SendMessage(_stateMachine.Chat, chatGptResponce);

            JobDone?.Invoke();
        }

        public override async Task Execute(Message message) { }

        public override async Task Execute(Update update) { }

        public override void ExitState() { }
    }
}
