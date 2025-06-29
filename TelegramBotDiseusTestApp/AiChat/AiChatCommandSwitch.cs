﻿using Mindee.Product.DriverLicense;
using Mindee.Product.InternationalId;
using Telegram.Bot;

namespace TelegramBotDiseusTestApp.AiChat
{
    internal partial class AiChat
    {
        private async Task ExecuteAiCommand(string command)
        {
            if (Enum.TryParse<AiCommand>(command, out var parsedCommand))
            {
                switch (parsedCommand)
                {
                    case AiCommand.ScanDocumentPhotos:
                        await ScanDocuments();
                        break;

                    case AiCommand.ShowDocumentsToUser:
                        await ShowDocumentsToUser();
                        break;

                    case AiCommand.ConfirmDocumentPhotos:
                        await UserConfirmedDocuments();
                        break;

                    case AiCommand.ConfirmPrice:
                        await UserAgreedToPay();
                        break;

                    case AiCommand.CreateInsuranse:
                        await CreateInsurance();
                        break;

                    case AiCommand.ClearData:
                        await ClearData();
                        break;
                }
            }
            else
            {
                Console.WriteLine("Unknown command");
            }
        }

        #region commands
        private async Task ScanDocuments()
        {
            _passport = await _mindeeService.GetIdData(_passportPhotoPath);
            _driverLicense = await _mindeeService.GetDriverLicenseData(_driverLicensePhotoPath);

            Console.WriteLine($"passport: {_passport}");
            Console.WriteLine($"driverLicense: {_driverLicense}");

            await HandleGroqRespond(_groqService.TalkToChat("[DocumentsWasScanned]", _userCurrentData, _chatHistory));
        }

        private async Task ShowDocumentsToUser()
        {
            await _bot.SendMessage(_chat.Id, _passport.Prediction.ToString());
            await _bot.SendMessage(_chat.Id, _driverLicense.Prediction.ToString());

            await HandleGroqRespond(_groqService.TalkToChat("[Documents shoved to user]", _userCurrentData, _chatHistory));
        }

        private async Task UserConfirmedDocuments()
        {
            _userCurrentData.PhotosConfirmed = true;
            await HandleGroqRespond(_groqService.TalkToChat("[UserConfirmedDocuments]", _userCurrentData, _chatHistory));
        }

        private async Task UserAgreedToPay()
        {
            _userCurrentData.PriceConfirmed = true;
            await HandleGroqRespond(_groqService.TalkToChat("[UserAgreedToPay]", _userCurrentData, _chatHistory));
        }

        private async Task CreateInsurance()
        {
            await HandleGroqRespond(_groqService.TalkToChat($"Generate a dummy insurance policy based on data:\n" +
                $"{_passport.Prediction.ToString()}\n" +
                $"{_driverLicense.Prediction.ToString()}"));

            JobDone?.Invoke(_chat.Id);
        }

        private async Task ClearData()
        {
            _telegramService.DeletePhoto(_passportPhotoPath);
            _telegramService.DeletePhoto(_driverLicensePhotoPath);

            _userCurrentData.PassportPhotoUploaded = false;
            _userCurrentData.DriverLicensePhotoUploaded = false;
            _userCurrentData.PhotosConfirmed = false;
            _userCurrentData.PriceConfirmed = false;

            _passportPhotoPath = "";
            _driverLicensePhotoPath = "";

            _passport = new InternationalIdV2();
            _driverLicense = new DriverLicenseV1();

            await HandleGroqRespond(_groqService.TalkToChat("[Data cleared]", _userCurrentData, _chatHistory));
        }
        #endregion
    }
}
