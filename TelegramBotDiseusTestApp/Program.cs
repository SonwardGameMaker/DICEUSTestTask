using GroqNet.ChatCompletions;
using Telegram.Bot;
using Telegram.Bot.Types;
using TelegramBotDiseusTestApp.AiChat;
using TelegramBotDiseusTestApp.DTOs.Options;
using TelegramBotDiseusTestApp.Services;

var builder = WebApplication.CreateBuilder(args);
var port = Environment.GetEnvironmentVariable("PORT") ?? "8080";
builder.WebHost.UseUrls($"http://0.0.0.0:{port}");

builder.Services.AddSingleton<ITelegramBotClient>(_ =>
    new TelegramBotClient(Environment.GetEnvironmentVariable("TELEGRAM_BOT_API_KEY")));

builder.Services.Configure<AiChatManagerOptions>(options =>
{
    options.MaxNumberOfChats = 5;
});

builder.Services.AddSingleton<AiChatManager>();

builder.Services.AddSingleton<TelegramDataTransferService>();

builder.Services.AddSingleton(_ =>
    new MindeeService(Environment.GetEnvironmentVariable("MINDEE_API_KEY")));

builder.Services.AddSingleton(_ =>
    new GroqService(Environment.GetEnvironmentVariable("GROQ_API_KEY"), GroqModel.LLaMA3_70b));

var app = builder.Build();

app.MapMethods("/", new[] { "GET", "HEAD" }, () => Results.Ok());

app.MapPost("/webhook", async (
    Update update,
    ITelegramBotClient bot,
    AiChatManager aiChatManager,
    TelegramDataTransferService telegramService,
    MindeeService mindeeService,
    GroqService groqService) =>
{
    await aiChatManager.TalkToChat(update);
    return Results.Ok();
});

Console.WriteLine("Bot is running");
app.Run();