using Mindee;
using Mindee.Input;
using Mindee.Product.DriverLicense;
using Mindee.Product.Passport;

namespace TelegramBotDiseusTestApp
{
    internal class MindeeService
    {
        private string _apiKey;

        public MindeeService(string apiKey)
        {
            _apiKey = apiKey;
        }

        public async Task<PassportV1> GetPassportData(string filePath)
        {
            MindeeClient mindeeClient = new MindeeClient(_apiKey);
            var inputSource = new LocalInputSource(filePath);
            var response = await mindeeClient.ParseAsync<PassportV1>(inputSource);

            return response.Document.Inference;
        }

        public async Task<DriverLicenseV1> GetDriverLicenseData(string filePath)
        {
            MindeeClient mindeeClient = new MindeeClient(_apiKey);
            var inputSource = new LocalInputSource(filePath);
            var response = await mindeeClient.EnqueueAndParseAsync<DriverLicenseV1>(inputSource);

            return response.Document.Inference;
        }
    }
}
