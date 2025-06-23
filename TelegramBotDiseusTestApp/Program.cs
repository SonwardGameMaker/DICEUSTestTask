using GroqNet.ChatCompletions;
using Telegram.Bot;
using Telegram.Bot.Types;
using TelegramBotDiseusTestApp.AiChat;
using TelegramBotDiseusTestApp.DTOs.Options;
using TelegramBotDiseusTestApp.Services;

var builder = WebApplication.CreateBuilder(args);

// --- Configuration ---
var port = Environment.GetEnvironmentVariable("PORT") ?? "8080";
var telegramApiKey = Environment.GetEnvironmentVariable("TELEGRAM_BOT_API_KEY");
var mindeeApiKey = Environment.GetEnvironmentVariable("MINDEE_API_KEY");
var groqApiKey = Environment.GetEnvironmentVariable("GROQ_API_KEY");

// --- Host ---
builder.WebHost.UseUrls($"http://0.0.0.0:{port}");
    
// --- Services ---
builder.Services.AddSingleton<ITelegramBotClient>(_ =>
    new TelegramBotClient(telegramApiKey));

builder.Services.Configure<AiChatManagerOptions>(options =>
{
    options.MaxNumberOfChats = 5;
});

builder.Services.AddSingleton<AiChatManager>();
builder.Services.AddSingleton<TelegramDataTransferService>();
builder.Services.AddSingleton(_ => new MindeeService(mindeeApiKey));
builder.Services.AddSingleton(_ => new GroqService(groqApiKey, GroqModel.LLaMA3_70b));

// --- App ---
var app = builder.Build();

app.MapMethods("/", new[] { "GET", "HEAD" }, () => Results.Ok());

app.MapPost("/webhook", async (
    Update update,
    ITelegramBotClient bot,
    AiChatManager aiChatManager) =>
{
    await aiChatManager.TalkToChat(update);
    return Results.Ok();
});

Console.WriteLine("Bot is running...");
app.Run();