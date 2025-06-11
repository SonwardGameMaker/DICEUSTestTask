using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TelegramBotDiseusTestApp
{
    internal class BotResponseData
    {
        public string GreetingsResponse { get; set; }
        public string PassportRequest { get; set; }
        public string DriverLicenceRequest { get; set; }

        public string DidntSendPassportResponse { get; set; }
        public string DidntSendDriverLicenseResponse { get; set; }
        public string Cancel { get; set; }
        public string Confirm { get; set; }
        public string Ok { get; set; }

        public static readonly BotResponseData TempResponceData = new BotResponseData 
        { 
            GreetingsResponse = "I'm your Car Insurance Assistant Bot. I'm here to help you purchase car insurance quickly and easily.\r\nLet's get started with the process!",
            PassportRequest = "To proceed, please send a clear photo of your passport",
            DriverLicenceRequest = "Now, please send a clear photo of your vehicle identification document",

            DidntSendPassportResponse = "Please send a clear photo of your passport",
            DidntSendDriverLicenseResponse = "Please send a clear photo of your vehicle identification document",
            Cancel = "Cancel",
            Confirm = "Confirm",
            Ok = "OK"
        };
    }
}
