namespace TelegramBotDiseusTestApp.DTOs
{
    internal class BotResponseData
    {
        public string GreetingsResponse { get; set; }
        public string PassportRequest { get; set; }
        public string DriverLicenceRequest { get; set; }
        public string DataConfirmation { get; set; }
        public string PriceQuotaionRequest { get; set; }
        public string StrongPriceQuotaionRequest { get; set; }

        public string DidntSendPassportResponse { get; set; }
        public string DidntSendDriverLicenseResponse { get; set; }

        public string ToMuchTokenUseWarning { get; set; }
        public string NoAwaliableServices {  get; set; }

        public string StartCommand { get; set; }
        public string Cancel { get; set; }
        public string Confirm { get; set; }
        public string Ok { get; set; }

        public static readonly BotResponseData DefaultResponceData = new BotResponseData 
        { 
            GreetingsResponse = "I'm your Car Insurance Assistant Bot. I'm here to help you purchase car insurance quickly and easily.\r\nLet's get started with the process!",
            PassportRequest = "To proceed, please send a clear photo of your passport",
            DriverLicenceRequest = "Now, please send a clear photo of your vehicle identification document",
            DataConfirmation = "Confirm your data:",
            PriceQuotaionRequest = "The fixed price for your car insurance is $100.\r\nDo you agree with this price?",
            StrongPriceQuotaionRequest = "Sorry, but the fixed price for your car insurance is $100.\r\nDo you agree with this price?",

            DidntSendPassportResponse = "Please send a clear photo of your passport",
            DidntSendDriverLicenseResponse = "Please send a clear photo of your vehicle identification document",

            ToMuchTokenUseWarning = "Warning: Too many tokens used. Earlier messages will be forgotten.",
            NoAwaliableServices = "No awaliable services rigght now. Please, try to use our bot later",

            StartCommand = "/start",
            Cancel = "Cancel",
            Confirm = "Confirm",
            Ok = "OK"
        };
    }
}
