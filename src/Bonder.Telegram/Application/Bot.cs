using Bonder.Calculation.Grpc;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;

namespace Application;
public sealed class Bot
{
    private static readonly char[] _splitters = [',', ' ', '\n'];

    private readonly string _token;
    private readonly ITelegramBotClient _bot;
    private readonly IServiceScopeFactory _scopeFactory;

    public Bot(string token, IServiceScopeFactory scopeFactory)
    {
        _token = token;
        _scopeFactory = scopeFactory;

        _bot = new TelegramBotClient(_token);

        _bot.StartReceiving(UpdateAsync, ErrorAsync);
    }

    public async Task UpdateAsync(ITelegramBotClient bot, Update update, CancellationToken token)
    {
        var text = update?.Message?.Text?.ToUpper();

        if (string.IsNullOrEmpty(text))
        {
            return;
        }

        var action = text.Split(_splitters)[0] switch
        {
            "/START" => HandleStartAsync(update, token),
            "/TOP_BONDS" => HandleTopBondsAsync(bot, update, token),
            _ => DoNothingAsync()
        };

        await action;
    }

    public async Task ErrorAsync(ITelegramBotClient bot, Exception exception, CancellationToken token)
    {

    }

    public async Task HandleStartAsync(Update update, CancellationToken token)
    {
        await _bot.SendTextMessageAsync
        (
            chatId: update.Message.Chat.Id,
            text: Printer.GetStartText(update),
            parseMode: ParseMode.Html,
            replyToMessageId: update.Message.MessageId,
            cancellationToken: token
        );
    }

    public async Task HandleTopBondsAsync(ITelegramBotClient bot, Update update, CancellationToken token)
    {
        await using var scope = _scopeFactory.CreateAsyncScope();

        var grpcClient = scope.ServiceProvider.GetRequiredService<CalculationService.CalculationServiceClient>();

        var bonds = await grpcClient.GetCurrentBondsAsync(new Google.Protobuf.WellKnownTypes.Empty(), cancellationToken: token);

        await _bot.SendTextMessageAsync
        (
            chatId: update.Message.Chat.Id,
            text: Printer.GetTopBondsText(bonds.Bonds),
            parseMode: ParseMode.Html,
            replyToMessageId: update.Message.MessageId,
            cancellationToken: token
        );
    }

    public static Task DoNothingAsync()
    { 
        return Task.CompletedTask;
    }
}
