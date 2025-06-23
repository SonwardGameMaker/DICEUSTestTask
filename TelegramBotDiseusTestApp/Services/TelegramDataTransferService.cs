using Telegram.Bot;
using Telegram.Bot.Types;

namespace TelegramBotDiseusTestApp.Services
{
    internal class TelegramDataTransferService
    {
        private string _downloadPath;
        private ITelegramBotClient _bot;

        public TelegramDataTransferService(ITelegramBotClient bot)
        {
            _bot = bot;
            _downloadPath = AppContext.BaseDirectory + "Download";
        }

        public async Task<string> GetPhoto(Message message, DocumentType documentType)
        {
            if (message.Photo == null) return null;

            var fileId = message.Photo[^1].FileId;
            var tgFile = await _bot.GetFile(fileId);

            CreateDownloadDirectory();
            var filePath = _downloadPath + "\\User" + message.Chat.Id + documentType.ToString() + ".jpg";
            await using var stream = File.Create(filePath);
            await _bot.DownloadFile(tgFile, stream);
            return filePath;
        }

        public bool DeletePhoto(string photoPath)
        {
            if (File.Exists(photoPath))
            {
                File.Delete(photoPath);
                return true;
            }
            return false;
        }

        private void CreateDownloadDirectory()
        {
            if (!Directory.Exists(_downloadPath))
                Directory.CreateDirectory(_downloadPath);
        }
    }
}
