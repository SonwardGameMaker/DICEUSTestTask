using GroqNet.ChatCompletions;
using Telegram.Bot;
using Telegram.Bot.Types;
using TelegramBotDiseusTestApp.FiniteStateMachine;
using TelegramBotDiseusTestApp.Services;

internal class Program
{
    private static async Task Main(string[] args)
    {
        var listener = new HttpListener();
        listener.Prefixes.Add($"http://*:{Environment.GetEnvironmentVariable("PORT") ?? "3000"}/");
        listener.Start();

        _ = Task.Run(() =>
        {
            while (true)
            {
                var ctx = listener.GetContext(); // ніколи не отримає, просто тримає порт відкритим
                var res = ctx.Response;
                res.StatusCode = 200;
                res.Close();
            }
        });

        TelegramBotClient bot;
        CancellationTokenSource cts;
        User me;

        StateMachineManager stateMachineManager;
        TelegramDataTransferService telegramService;
        MindeeService mindeeService;
        GroqService groqService;

        cts = new CancellationTokenSource();
        bot = new TelegramBotClient(Environment.GetEnvironmentVariable("TELEGRAM_BOT_API_KEY"), cancellationToken: cts.Token);
        me = await bot.GetMe();

        stateMachineManager = new StateMachineManager(5);
        telegramService = new TelegramDataTransferService(bot);
        mindeeService = new MindeeService(Environment.GetEnvironmentVariable("MINDEE_API_KEY"));
        groqService = new GroqService(Environment.GetEnvironmentVariable("GROQ_API_KEY"), GroqModel.LLaMA3_70b);
        stateMachineManager.Init(bot, telegramService, mindeeService, groqService);

        bot.OnMessage += stateMachineManager.Execute;
        bot.OnUpdate += stateMachineManager.Execute;

        Console.WriteLine($"@{me.Username} is running... Press Enter to terminate");
        Console.ReadLine();
        cts.Cancel();
    }
}