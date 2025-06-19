namespace TelegramBotDiseusTestApp.DTOs
{
    public class UserCurrentData
    {
        public UserCurrentData(string userName)
        {
            UserName = userName;

            PassportPhotoUploaded = false;
            DriverLicensePhotoUploaded = false;
            PhotosConfirmed = false;
            PriceConfirmed = false;
        }

        public string UserName { get; private set; }
        public bool PassportPhotoUploaded {  get; set; }
        public bool DriverLicensePhotoUploaded {  get; set; }
        public bool PhotosConfirmed {  get; set; }
        public bool PriceConfirmed {  get; set; }
    }
}
