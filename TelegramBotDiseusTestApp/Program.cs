using Mindee.Product.Passport;
using Telegram.Bot;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;
using TelegramBotDiseusTestApp;
using TelegramBotDiseusTestApp.DTOs;

internal class Program
{
    private static TelegramBotClient bot;
    private static CancellationTokenSource cts;
    private static User me;

    private static TelegramDataTransferService _telegramDataTransfer;
    private static MindeeService _mindeeService;
    private static OpenAiService _openAiService;

    static string photoPath;
    static bool checkForPhoto = false;

    private static async Task Main(string[] args)
    {
        cts = new CancellationTokenSource();
        bot = new TelegramBotClient("7673635566:AAEBa1PwtHGKHFBFHHTyInMrVwThbj_UcL0", cancellationToken: cts.Token);
        me = await bot.GetMe();

        _telegramDataTransfer = new TelegramDataTransferService(bot);
        _mindeeService = new MindeeService("7d7e630731884ad4aab30abce5651b3a");
        _openAiService = new OpenAiService("gpt-3.5-turbo", "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");

        //TelegramBotTest();
        //ReadPhotoData();
        SpeakWithChatGPT();
    }

    private static async void TelegramBotTest()
    {
        bot.OnError += OnError;
        bot.OnMessage += OnMessage;
        bot.OnUpdate += OnUpdate;

        Console.WriteLine($"@{me.Username} is running... Press Enter to terminate");
        Console.ReadLine();
        cts.Cancel();

        async Task OnError(Exception exception, HandleErrorSource source)
        {
            Console.WriteLine(exception); // just dump the exception to the console
        }

        async Task OnMessage(Message message, UpdateType type)
        {
            if (message.Text == "/start")
            {
                await bot.SendMessage(message.Chat, BotResponseData.TempResponceData.GreetingsResponse);
                await bot.SendMessage(message.Chat, BotResponseData.TempResponceData.PassportRequest);
            }

            if (message.Photo != null)
            {

            }

        }

        async Task OnUpdate(Update update)
        {
            if (update is { CallbackQuery: { } query }) // non-null CallbackQuery
            {
                await bot.AnswerCallbackQuery(query.Id, $"You picked {query.Data}");
                await bot.SendMessage(query.Message!.Chat, $"User {query.From} clicked on {query.Data}");
            }
        }
    }

    private static async void ReadPhotoData()
    {
        bot.OnMessage += OnMessage;

        Console.WriteLine($"@{me.Username} is running... Press Enter to terminate");
        Console.ReadLine();
        cts.Cancel();

        async Task OnMessage(Message message, UpdateType updateType)
        {
            if (message.Text == "/start")
            {
                await bot.SendMessage(message.Chat, "Please, upload passport photo:");
            }

            if (message.Photo != null)
            {
                photoPath = await _telegramDataTransfer.GetPhoto(message, DocumentType.Passport);

                if (photoPath != null)
                {
                    await bot.SendMessage(message.Chat, photoPath);
                    var mindeeRespond = await _mindeeService.GetPassportData(photoPath);
                    await bot.SendMessage(message.Chat, "Result of extraction:\n" + mindeeRespond.Prediction.ToString());

                    string chatGPTRespond = await _openAiService.TextToChat($"Generate a dummy insurance policy document on person {mindeeRespond.Prediction.ToString()}");
                    await bot.SendMessage(message.Chat, "Chat respond:\n" + chatGPTRespond);
                }
                else
                    await bot.SendMessage(message.Chat, "No photo found");
            }
        }
    }

    private static async void SpeakWithChatGPT()
    {
        bot.OnMessage += OnMessage;

        Console.WriteLine($"@{me.Username} is running... Press Enter to terminate");
        Console.ReadLine();
        cts.Cancel();

        async Task OnMessage(Message message, UpdateType updateType)
        {
            string chatGPTRespond = await _openAiService.TextToChat($"{message.Text}");
            await bot.SendMessage(message.Chat, "Chat respond:\n" + chatGPTRespond);
        }
    }
}