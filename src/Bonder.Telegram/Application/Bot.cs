﻿using Bonder.Calculation.Grpc;
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

namespace Application;

public sealed class Filters
{
    public DateTime? StartDate { get; set; }
    public decimal PriceFrom { get; set; } = 0;
    public decimal PriceTo { get; set; } = decimal.MaxValue;
    public decimal RatingFrom { get; set; } = 0;
    public decimal RatingTo { get; set; } = 10;
    public DateOnly DateFrom { get; set; } = DateOnly.FromDateTime(DateTime.Now);
    public DateOnly DateTo { get; set; } = DateOnly.MaxValue;
    public bool IncludeUnknownRatings { get; set; } = true;

    public bool CanProcess => StartDate != null && StartDate.Value.AddMinutes(1) > DateTime.Now;
}

public enum State
{
    Start,
    GettingPriceFrom,
    GettingPriceTo,
    GettingRatingFrom,
    GettingRatingTo,
    GettingDateFrom,
    GettingDateTo,
    Finished
}

public enum Trigger
{
    GetPriceFrom,
    GetPriceTo,
    GetRatingFrom,
    GetRatingTo,
    GetDateFrom,
    GetDateTo,
    Finish
}

public sealed class Bot
{
    private static readonly char[] _splitters = [',', ' ', '\n'];
    private static readonly ConcurrentDictionary<string, Filters> _filters = new();

    private readonly string _token;
    private readonly ITelegramBotClient _bot;
    private readonly IServiceScopeFactory _scopeFactory;

    private Message _message;
    private readonly StateMachine<State, Trigger> _machine;

    public Bot(string token, IServiceScopeFactory scopeFactory)
    {
        var machine = new StateMachine<State, Trigger>(State.Start);

        machine.Configure(State.Start)
            .Permit(Trigger.GetPriceFrom, State.GettingPriceFrom);

        machine.Configure(State.GettingPriceFrom)
            .OnEntryAsync(x => GetPriceFrom())
            .Permit(Trigger.GetPriceFrom, State.GettingPriceTo);

        _machine = machine;

        _token = token;
        _scopeFactory = scopeFactory;

        _bot = new TelegramBotClient(_token);

        _bot.StartReceiving(UpdateAsync, ErrorAsync);
    }

    private async Task UpdateAsync(ITelegramBotClient bot, Update update, CancellationToken token)
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

        if (text.Any(x => !char.IsAscii(x)))
        {
            await HandleFiltersAsync(message, token);

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

    private async Task HandleFiltersAsync(Message message, CancellationToken token)
    {
       var text = 
    }

    private async Task GetPriceFrom()
    {
        await _bot.SendTextMessageAsync
        (
            _message.Chat.Id,
            "Ок"
        );

        if(!_filters.TryGetValue(_message.Chat.Username, out var filter))
        {
            filter = new Filters();
        }

        filter.PriceFrom = decimal.Parse(_message.Text.Split(":")[^1]);
    }

    private async Task HandleTopBondsNoFilters(Message message, CancellationToken token)
    {
        await using var scope = _scopeFactory.CreateAsyncScope();

        var grpcClient = scope.ServiceProvider.GetRequiredService<CalculationService.CalculationServiceClient>();

        var bonds = await grpcClient.GetCurrentBondsAsync(new Google.Protobuf.WellKnownTypes.Empty(), cancellationToken: token);

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
        if (!_filters.TryGetValue(message.Chat.Username, out var filters))
        {
            filters = new Filters();
        }

        filters.StartDate = DateTime.Now;

        _filters.TryAdd(message.Chat.Username, filters);

        await _bot.SendTextMessageAsync
        (
            message.Chat.Id, 
            $"Введите /skip чтобы пропустить фильтр (будет установлено значение по умолчанию)", 
            cancellationToken: token
        );

        await _bot.SendTextMessageAsync
        (
            message.Chat.Id,
            "(Формат \"Цена от: *<b>значение</b>*\")",
            parseMode: ParseMode.Html,
            cancellationToken: token
        );

        await _bot.SendTextMessageAsync
        (
            message.Chat.Id, "Введите \"Цену от\":",
            cancellationToken: token
        );
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
