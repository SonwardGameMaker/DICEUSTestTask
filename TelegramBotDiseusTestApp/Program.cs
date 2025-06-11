using Telegram.Bot;
using Telegram.Bot.Types;
using TelegramBotDiseusTestApp.FiniteStateMachine;
using TelegramBotDiseusTestApp.Services;

internal class Program
{
    private static async Task Main(string[] args)
    {
        TelegramBotClient bot;
        CancellationTokenSource cts;
        User me;

        StateMachineManager stateMachineManager;
        TelegramDataTransferService telegramService;
        MindeeService mindeeService;
        OpenAiService openAiService;

        cts = new CancellationTokenSource();
        bot = new TelegramBotClient("7673635566:AAEBa1PwtHGKHFBFHHTyInMrVwThbj_UcL0", cancellationToken: cts.Token);
        me = await bot.GetMe();

        stateMachineManager = new StateMachineManager(5);
        telegramService = new TelegramDataTransferService(bot);
        mindeeService = new MindeeService("7d7e630731884ad4aab30abce5651b3a");
        openAiService = new OpenAiService("gpt-3.5-turbo", "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");
        stateMachineManager.Init(bot, telegramService, mindeeService, openAiService);

        bot.OnMessage += stateMachineManager.Execute;
        bot.OnUpdate += stateMachineManager.Execute;

        Console.WriteLine($"@{me.Username} is running... Press Enter to terminate");
        Console.ReadLine();
        cts.Cancel();
    }
}