using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;

namespace TelegramBotDiseusTestApp.FiniteStateMachine.ChatStates.Concrete
{
    internal class PriceQuotationState : ChatState
    {
        BotResponseData _response;

        public PriceQuotationState(ChatStateMachine chatStateMachine) : base(chatStateMachine)
        {
            _response = BotResponseData.TempResponceData;
        }

        public override async void EnterState()
        {
            // TODO refactor
            await _stateMachine.Bot.SendMessage(_stateMachine.Chat, "Price Quotation message", replyMarkup: new InlineKeyboardButton[] { _response.Cancel, _response.Ok });
        }

        public override async Task Execute(Message message)
            => await _stateMachine.Bot.SendMessage(_stateMachine.Chat, "Please read the price quotation");

        public override async Task Execute(Update update)
        {
            if (update is { CallbackQuery: { } query })
            {
                if (query.Data == _response.Cancel)
                {
                    await _stateMachine.Bot.AnswerCallbackQuery(query.Id, $"You denied payment");
                    await _stateMachine.Bot.SendMessage(_stateMachine.Chat, "Very inisisting Price Quotation message", replyMarkup: new InlineKeyboardButton[] { _response.Cancel, _response.Ok });
                }
                else if (query.Data == _response.Ok)
                {
                    await _stateMachine.Bot.AnswerCallbackQuery(query.Id, $"You agreed with payment");
                    _stateMachine.ChangeSate<InsurancePolicyIssuanceState>();
                }
            }
        }

        public override void ExitState() { }
    }
}
