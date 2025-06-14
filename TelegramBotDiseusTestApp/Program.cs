using GroqNet.ChatCompletions;
using Microsoft.AspNetCore.Http;
using System.Net;
using System.Text.Json.Serialization;
using Telegram.Bot;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;
using TelegramBotDiseusTestApp.FiniteStateMachine;
using TelegramBotDiseusTestApp.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.ConfigureHttpJsonOptions(o =>
    o.SerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull);

builder.Services.AddSingleton<ITelegramBotClient>(_ =>
    new TelegramBotClient(Environment.GetEnvironmentVariable("TELEGRAM_BOT_API_KEY")));

builder.Services.AddSingleton<StateMachineManager>(_ => new StateMachineManager(5));
builder.Services.AddSingleton<TelegramDataTransferService>();
builder.Services.AddSingleton<MindeeService>(_ =>
    new MindeeService(Environment.GetEnvironmentVariable("MINDEE_API_KEY")));
builder.Services.AddSingleton<GroqService>(_ =>
    new GroqService(Environment.GetEnvironmentVariable("GROQ_API_KEY"), GroqModel.LLaMA3_70b));

var app = builder.Build();

app.MapPost("/webhook", async (HttpContext ctx,
                               TelegramBotClient bot,
                               StateMachineManager sm,
                               TelegramDataTransferService tg,
                               MindeeService mindee,
                               GroqService groq,
                               CancellationToken ct) =>
{
    var update = await ctx.Request.ReadFromJsonAsync<Update>(cancellationToken: ct);
    if (update is null) return Results.BadRequest();
    sm.Init(bot, tg, mindee, groq);
    await sm.Execute(update.Message, update.Type);
    return Results.Ok();
});

app.Run();