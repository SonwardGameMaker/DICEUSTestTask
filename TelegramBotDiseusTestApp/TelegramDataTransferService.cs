using Telegram.Bot;
using Telegram.Bot.Types;

namespace TelegramBotDiseusTestApp
{
    internal class TelegramDataTransferService
    {
        private string _downloadPath;
        private TelegramBotClient _bot;

        public TelegramDataTransferService(TelegramBotClient bot)
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
            string filePath = _downloadPath + "\\User" + message.Chat.Id + documentType.ToString() + ".jpg";
            await using var stream = File.Create(filePath);
            await _bot.DownloadFile(tgFile, stream);
            return filePath;
        }

        private void CreateDownloadDirectory()
        {
            if (!Directory.Exists(_downloadPath))
                Directory.CreateDirectory(_downloadPath);
        }
    }
}
