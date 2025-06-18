using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;

namespace TelegramBotDiseusTestApp.FiniteStateMachine.ChatStates.Concrete
{
    internal class PriceQuotationState : ChatState
    {

        public PriceQuotationState(ChatStateMachine chatStateMachine) : base(chatStateMachine) { }

        public override async void EnterState()
            => await _stateMachine.Bot.SendMessage(_stateMachine.Chat, _response.PriceQuotaionRequest, 
                replyMarkup: new InlineKeyboardButton[] { _response.Cancel, _response.Ok });

        public override async Task Execute(Message message)
        {
            if (message.Text != null)
                await AskGroq(message.Text);
            else
                await _stateMachine.Bot.SendMessage(_stateMachine.Chat, "Please read the price quotation");
        }

        public override async Task Execute(Update update)
        {
            if (update is { CallbackQuery: { } query })
            {
                if (query.Data == _response.Cancel)
                {
                    await _stateMachine.Bot.AnswerCallbackQuery(query.Id, "You denied payment");
                    await _stateMachine.Bot.SendMessage(_stateMachine.Chat, _response.StrongPriceQuotaionRequest, replyMarkup: new InlineKeyboardButton[] { _response.Cancel, _response.Ok });
                }
                else if (query.Data == _response.Ok)
                {
                    await _stateMachine.Bot.AnswerCallbackQuery(query.Id, "You agreed with payment");
                    _stateMachine.ChangeSate<InsurancePolicyIssuanceState>();
                }
            }
        }

        public override void ExitState() { }
    }
}
