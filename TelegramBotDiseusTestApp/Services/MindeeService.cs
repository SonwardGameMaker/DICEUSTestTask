using Mindee;
using Mindee.Input;
using Mindee.Product.DriverLicense;
using Mindee.Product.InternationalId;
using Mindee.Product.Passport;

namespace TelegramBotDiseusTestApp.Services
{
    internal class MindeeService
    {
        private MindeeClient _mindeeClient;

        public MindeeService(string apiKey)
        {
            _mindeeClient = new MindeeClient(apiKey);
        }

        public async Task<InternationalIdV2> GetIdData(string filePath)
        {
            var inputSource = new LocalInputSource(filePath);
            var response = await _mindeeClient.EnqueueAndParseAsync<InternationalIdV2>(inputSource);

            return response.Document.Inference;
        }
        public async Task<InternationalIdV2> GetIdDataMock(string filePath)
        {
            var response = new InternationalIdV2();

            return response;
        }

        public async Task<PassportV1> GetPassportData(string filePath)
        {
            var inputSource = new LocalInputSource(filePath);
            var response = await _mindeeClient.ParseAsync<PassportV1>(inputSource);

            return response.Document.Inference;
        }

        public async Task<DriverLicenseV1> GetDriverLicenseData(string filePath)
        {
            var inputSource = new LocalInputSource(filePath);
            var response = await _mindeeClient.EnqueueAndParseAsync<DriverLicenseV1>(inputSource);

            return response.Document.Inference;
        }
        public async Task<DriverLicenseV1> GetDriverLicenseDataMock(string filePath)
        {
            var response = new DriverLicenseV1();

            return response;
        }
    }
}
