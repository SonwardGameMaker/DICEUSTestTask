using GroqNet.ChatCompletions;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Telegram.Bot;
using Telegram.Bot.Types;
using TelegramBotDiseusTestApp.FiniteStateMachine;
using TelegramBotDiseusTestApp.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSingleton<ITelegramBotClient>(_ =>
    new TelegramBotClient(Environment.GetEnvironmentVariable("TELEGRAM_BOT_API_KEY")));

builder.Services.AddSingleton<StateMachineManager>();
builder.Services.AddSingleton<TelegramDataTransferService>();
builder.Services.AddSingleton<MindeeService>(_ =>
    new MindeeService(Environment.GetEnvironmentVariable("MINDEE_API_KEY")));
builder.Services.AddSingleton<GroqService>(_ =>
    new GroqService(Environment.GetEnvironmentVariable("GROQ_API_KEY"), GroqModel.LLaMA3_70b));

var app = builder.Build();

app.MapMethods("/", new[] { "GET", "HEAD" }, () => Results.Ok());

app.MapPost("/webhook", async (Update update,
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

app.Run();