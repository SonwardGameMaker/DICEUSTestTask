using Mindee.Product.DriverLicense;
using Mindee.Product.InternationalId;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;
using TelegramBotDiseusTestApp.Services;

namespace TelegramBotDiseusTestApp.FiniteStateMachine.ChatStates.Concrete
{
    internal class DriverLicenseRequirementState : ChatState
    {
        private bool _photoWasScaned;
        private MindeeService _mindeeService;
        
        public DriverLicenseRequirementState(ChatStateMachine chatStateMachine, MindeeService mindeeService) : base(chatStateMachine)
        {
            _photoWasScaned = false;
            _mindeeService = mindeeService;
        }

        public override async void EnterState()
            => await _stateMachine.Bot.SendMessage(_stateMachine.Chat, _response.DriverLicenceRequest);

        public override async Task Execute(Message message)
        {
            if (message.Photo != null)
            {
                if (!_photoWasScaned)
                {
                    _stateMachine.DriverLicensePhotoPath = await _stateMachine.TelegramService.GetPhoto(message, DocumentType.DriverLicense);

                    _stateMachine.Passport = await _mindeeService.GetIdData(_stateMachine.PassportPhotoPath);
                    _stateMachine.DriverLicense = await _mindeeService.GetDriverLicenseData(_stateMachine.DriverLicensePhotoPath);
                    _photoWasScaned = true;

                    await _stateMachine.Bot.SendMessage(_stateMachine.Chat, _stateMachine.Passport.Prediction.ToString());
                    await _stateMachine.Bot.SendMessage(_stateMachine.Chat, _stateMachine.DriverLicense.Prediction.ToString());

                    await _stateMachine.Bot.SendMessage(_stateMachine.Chat, _response.DataConfirmation, replyMarkup: new InlineKeyboardButton[] { _response.Cancel, _response.Confirm });
                }
                else
                {
                    if (message.Text != null)
                        await AskGroq(message.Text);
                    else
                        await _stateMachine.Bot.SendMessage(_stateMachine.Chat, _response.DidntSendDriverLicenseResponse);
                }
            }
            else
            {
                if (message.Text != null)
                    await AskGroq(message.Text);
                else
                    await _stateMachine.Bot.SendMessage(_stateMachine.Chat, _response.DidntSendDriverLicenseResponse);
            }
                
        }

        public override async Task Execute(Update update)
        {
            if (update is { CallbackQuery: { } query }) 
            {
                if (query.Data == _response.Cancel)
                {
                    await _stateMachine.Bot.AnswerCallbackQuery(query.Id, $"You denied file data");
                    ClearData();
                    _stateMachine.ChangeSate<PassportRequirementState>();
                }
                else if (query.Data == _response.Confirm)
                {
                    await _stateMachine.Bot.AnswerCallbackQuery(query.Id, $"You confirmed file data");
                    _stateMachine.ChangeSate<PriceQuotationState>();
                }
            }
        }

        public override void ExitState() { }

        public void ClearData()
        {
            _photoWasScaned = false;

            _stateMachine.TelegramService.DeletePhoto(_stateMachine.PassportPhotoPath);
            _stateMachine.TelegramService.DeletePhoto(_stateMachine.DriverLicensePhotoPath);

            _stateMachine.PassportPhotoPath = "";
            _stateMachine.DriverLicensePhotoPath = "";

            _stateMachine.Passport = new InternationalIdV2();
            _stateMachine.DriverLicense = new DriverLicenseV1();
        }
    }
}
