using Bonder.Calculation.Grpc;
using Microsoft.Extensions.DependencyInjection;
using Stateless;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.PortableExecutable;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;
using Web.Services.Dto;

namespace Web.Services;

public sealed class Bot
{
    private static readonly char[] _splitters = [',', ' ', '\n']; 
    private static readonly StateDictionary _states = new();
    private static readonly StateMachineFactory _factory = new(_states);

    private readonly ITelegramBotClient _bot;
    private readonly CalculationService.CalculationServiceClient _grpcService;

    public Bot(ITelegramBotClient bot, CalculationService.CalculationServiceClient grpcService)
    {
        _bot = bot;
        _grpcService = grpcService;
    }

    public async Task HandleUpdateAsync(Update update, CancellationToken token)
    {
        var handler = update switch
        {
            { Message: { } message } => BotOnMessageReceived(message, token),
            { CallbackQuery: { } callbackQuery } => BotOnCallbackQueryReceived(callbackQuery, token),
            _ => DoNothingAsync()
        };

        await handler;
    }

    private async Task BotOnMessageReceived(Message message, CancellationToken token)
    {
        var text = message?.Text?.ToUpper();
        var sticker = message?.Sticker?.FileId;

        if (!string.IsNullOrEmpty(sticker))
        {
            await _bot.SendStickerAsync(message.Chat.Id, InputFile.FromFileId(sticker), cancellationToken: token);

            return;
        }

        if (string.IsNullOrEmpty(text))
        {
            return;
        }

        if (text.All(char.IsNumber))
        {
            await HandleFiltersAsync(message);

            return;
        }

        var action = text.Split(_splitters)[0] switch
        {
            "/START" => HandleStartAsync(message, token),
            "/TOP_BONDS" => HandleTopBondsAsync(message, token),
            _ => DoNothingAsync()
        };

        await action;
    }

    private async Task BotOnCallbackQueryReceived(CallbackQuery query, CancellationToken token)
    {
        var text = query?.Data?.ToUpper();

        if (string.IsNullOrEmpty(text))
        {
            return;
        }

        var action = text.Split(_splitters)[0] switch
        {
            "/TOP_BONDS_NO_FILTERS" => HandleTopBondsNoFilters(query.Message, token),
            "/TOP_BONDS_WITH_FILTERS" => HandleTopBondsWithFilters(query.Message, token),
            _ => DoNothingAsync()
        };

        await action;
    }

    private static async Task HandleFiltersAsync(Message message)
    {
        var state = _states.GetState(message);

        await state.StateMachine.FireAsync(state.Filters.NextTrigger);

        state.Filters.UpdateNextTrigger(state.Filters.NextTrigger);
    }

    private async Task HandleTopBondsNoFilters(Message message, CancellationToken token)
    {
        var bonds = await _grpcService.GetCurrentBondsAsync(new Google.Protobuf.WellKnownTypes.Empty(), cancellationToken: token);

        await _bot.SendTextMessageAsync
        (
            chatId: message.Chat.Id,
            text: Printer.GetTopBondsText(bonds.Bonds),
            parseMode: ParseMode.Html,
            replyToMessageId: message.MessageId,
            cancellationToken: token
        );
    }

    private async Task HandleTopBondsWithFilters(Message message, CancellationToken token)
    {
        var state = _states.GetState(message);

        state.Filters.StartDate = DateTime.Now;
        state.StateMachine ??= _factory.Create(_bot, message);

        await _bot.SendTextMessageAsync
        (
            message.Chat.Id,
            $"Введите /skip чтобы пропустить фильтр (будет установлено значение по умолчанию)",
            cancellationToken: token
        );

        await _bot.SendTextMessageAsync
        (
            message.Chat.Id,
            "Введите \"Цену от\":",
            cancellationToken: token
        );

        _states.Add(message.Chat.Username, state);
    }

    private async Task HandleStartAsync(Message message, CancellationToken token)
    {
        await _bot.SendTextMessageAsync
        (
            chatId: message.Chat.Id,
            text: Printer.GetStartText(message),
            parseMode: ParseMode.Html,
            replyToMessageId: message.MessageId,
            cancellationToken: token
        );
    }

    private async Task HandleTopBondsAsync(Message message, CancellationToken token)
    {
        var replyMarkup = new InlineKeyboardMarkup(new InlineKeyboardButton[][]
        {
            [InlineKeyboardButton.WithCallbackData("С фильтрами", "/TOP_BONDS_WITH_FILTERS")],
            [InlineKeyboardButton.WithCallbackData("Без фильтров", "/TOP_BONDS_NO_FILTERS")]
        });

        await _bot.SendTextMessageAsync
        (
            message.Chat.Id,
            "Выберите действие:",
            replyToMessageId: message.MessageId,
            replyMarkup: replyMarkup,
            cancellationToken: token
        );
    }

    private static Task DoNothingAsync()
    {
        return Task.CompletedTask;
    }

    private async Task ErrorAsync(ITelegramBotClient bot, Exception exception, CancellationToken token)
    {

    }
}
