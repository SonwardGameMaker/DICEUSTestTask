using GroqNet.ChatCompletions;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Telegram.Bot;
using Telegram.Bot.Types;
using TelegramBotDiseusTestApp.FiniteStateMachine;
using TelegramBotDiseusTestApp.Services;

var builder = WebApplication.CreateBuilder(args);
var port = Environment.GetEnvironmentVariable("PORT") ?? "8080";
builder.WebHost.UseUrls($"http://0.0.0.0:{port}");

builder.Services.AddSingleton<ITelegramBotClient>(_ =>
    new TelegramBotClient(Environment.GetEnvironmentVariable("TELEGRAM_BOT_API_KEY")));

builder.Services.AddSingleton<StateMachineManager>(_ => new StateMachineManager(5));

builder.Services.AddSingleton<TelegramDataTransferService>();

builder.Services.AddSingleton<MindeeService>(_ =>
    new MindeeService(Environment.GetEnvironmentVariable("MINDEE_API_KEY")));

builder.Services.AddSingleton<GroqService>(_ =>
    new GroqService(Environment.GetEnvironmentVariable("GROQ_API_KEY"), GroqModel.LLaMA3_70b));

var app = builder.Build();

app.MapMethods("/", new[] { "GET", "HEAD" }, () => Results.Ok());

app.MapPost("/webhook", async (
    Update update,
    ITelegramBotClient bot,
    StateMachineManager sm,
    TelegramDataTransferService tg,
    MindeeService mindee,
    GroqService groq) =>
{
    sm.Init(bot, tg, mindee, groq);
    await sm.Execute(update);
    return Results.Ok();
});

Console.WriteLine("Bot is running");
app.Run();