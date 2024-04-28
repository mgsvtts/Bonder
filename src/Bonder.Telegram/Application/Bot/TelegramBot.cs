using Application.Bot.Dto;
using Application.Helpers;
using Bonder.Calculation.Grpc;
using Mapster;
using Stateless;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;

namespace Application.Bot;

public sealed class TelegramBot
{
    private static readonly char[] _splitters = [',', ' ', '\n'];
    private static readonly StateDictionary _states = new();
    private static readonly StateMachineFactory _factory = new(_states);

    private readonly ITelegramBotClient _bot;
    private readonly CalculationService.CalculationServiceClient _grpcService;

    public TelegramBot(ITelegramBotClient bot, CalculationService.CalculationServiceClient grpcService)
    {
        _bot = bot;
        _grpcService = grpcService;
    }

    public async Task HandleUpdateAsync(Update update, CancellationToken token)
    {
        var handler = update switch
        {
            { Message: { } message } => ReceiveMessage(message, token),
            { CallbackQuery: { } callbackQuery } => ReceivedCallback(callbackQuery, token),
            _ => DoNothingAsync(update.Message, token)
        };

        await handler;
    }

    private async Task ReceiveMessage(Message message, CancellationToken token)
    {
        var sticker = message?.Sticker?.FileId;

        if (!string.IsNullOrEmpty(sticker))
        {
            await _bot.SendStickerAsync
            (
                message.Chat.Id,
                InputFile.FromFileId(sticker),
                replyToMessageId: message.MessageId,
                cancellationToken: token);

            return;
        }

        var state = _states.GetState(message);
        if (state.Filters != BondFilters.Default)
        {
            await HandleFiltersAsync(message, state);

            return;
        }

        var action = message?.Text?.ToUpper().Split(_splitters)[0] switch
        {
            "/START" => HandleStartAsync(message, token),
            "/TOP_BONDS" => HandleTopBondsAsync(message, token),
            "/DEVS" => HandleDevsAsync(message, token),
            _ => DoNothingAsync(message, token)
        };

        await action;
    }

    private async Task ReceivedCallback(CallbackQuery query, CancellationToken token)
    {
        var text = query?.Data?.ToUpper();

        if (string.IsNullOrEmpty(text))
        {
            return;
        }

        var action = text.Split(_splitters)[0] switch
        {
            "/TOP_BONDS_NO_FILTERS" => HandleTopBondsNoFiltersAsync(query.Message, token),
            "/TOP_BONDS_WITH_FILTERS" => HandleTopBondsWithFiltersAsync(query.Message, token),
            "/BONDS_WITH_UNKNOWN_RATINGS" => HandleBondsWithUnknownRatingsAsync(query.Message, true),
            "/BONDS_WITHOUT_UNKNOWN_RATINGS" => HandleBondsWithUnknownRatingsAsync(query.Message, false),
            "/DATEFROM_IS_TODAY" => HandleDateFromAsync(query.Message),
            "/DATETO_IS_MATURITY" => HandleDateToAsync(query.Message, DateToType.Maturity),
            "/DATETO_IS_OFFER" => HandleDateToAsync(query.Message, DateToType.Offer),
            "/DATETO_ONE_YEAR" => HandleDateToAsync(query.Message, DateToType.OneYear),
            "/DATETO_THREE_YEARS" => HandleDateToAsync(query.Message, DateToType.ThreeYears),
            "/DATETO_FIVE_YEARS" => HandleDateToAsync(query.Message, DateToType.FiveYears),
            "/DATETO_TEN_YEARS" => HandleDateToAsync(query.Message, DateToType.TenYears),
            _ => DoNothingAsync(query.Message, token)
        };

        await action;
    }

    private static async Task HandleDateFromAsync(Message message)
    {
        var state = _states.GetState(message);

        if (state.Filters == BondFilters.Default)
        {
            return;
        }

        state.Filters.DateFrom = DateOnly.FromDateTime(DateTime.Now);

        await _factory.FireAsync(new StateMachine<State, Trigger>.TriggerWithParameters<Message>(Trigger.Next), message, state.StateMachine);
    }

    private static async Task HandleDateToAsync(Message message, DateToType type)
    {
        var state = _states.GetState(message);

        if (state.Filters == BondFilters.Default)
        {
            return;
        }

        if (type == DateToType.Maturity)
        {
            state.Filters.DateToType = DateToType.Maturity;
        }
        else if (type == DateToType.Offer)
        {
            state.Filters.DateToType = DateToType.Offer;
        }
        else if (type == DateToType.OneYear)
        {
            state.Filters.DateTo = DateOnly.FromDateTime(DateTime.Now.AddYears(1));
        }
        else if (type == DateToType.ThreeYears)
        {
            state.Filters.DateTo = DateOnly.FromDateTime(DateTime.Now.AddYears(3));
        }
        else if (type == DateToType.FiveYears)
        {
            state.Filters.DateTo = DateOnly.FromDateTime(DateTime.Now.AddYears(5));
        }
        else if (type == DateToType.TenYears)
        {
            state.Filters.DateTo = DateOnly.FromDateTime(DateTime.Now.AddYears(10));
        }

        await _factory.FireAsync(new StateMachine<State, Trigger>.TriggerWithParameters<Message>(Trigger.Next), message, state.StateMachine);
    }

    private async Task HandleFiltersAsync(Message message, UserState state)
    {
        if (message.Text.Contains("/EXIT", StringComparison.CurrentCultureIgnoreCase))
        {
            await _bot.SendTextMessageAsync
            (
                message.Chat.Id,
                "Выход из режима фильтрации",
                replyToMessageId: message.MessageId
            );

            state.Filters = BondFilters.Default;

            return;
        }

        if (message.Text.Contains("/SKIP", StringComparison.CurrentCultureIgnoreCase))
        {
            await _factory.FireSkipAsync(message, state.StateMachine);
        }
        else
        {
            await _factory.FireAsync(new StateMachine<State, Trigger>.TriggerWithParameters<Message>(Trigger.Next), message, state.StateMachine);
        }
    }

    private async Task HandleTopBondsNoFiltersAsync(Message message, CancellationToken token)
    {
        await _bot.SendChatActionAsync
        (
            message.Chat.Id,
            ChatAction.Typing,
            cancellationToken: token
        );

        var bonds = await _grpcService.GetCurrentBondsAsync(BondFilters.Default.Adapt<Filters>(), cancellationToken: token);

        await _bot.SendTextMessageAsync
        (
            chatId: message.Chat.Id,
            text: Printer.GetTopBondsText(bonds.Bonds),
            parseMode: ParseMode.Html,
            replyToMessageId: message.MessageId,
            cancellationToken: token
        );
    }

    private static async Task HandleBondsWithUnknownRatingsAsync(Message message, bool value)
    {
        var state = _states.GetState(message);

        if (state.Filters == BondFilters.Default)
        {
            return;
        }

        state.Filters.IncludeUnknownRatings = value;

        await _factory.FireAsync(new StateMachine<State, Trigger>.TriggerWithParameters<Message>(Trigger.Next), message, state.StateMachine);
    }

    private async Task HandleTopBondsWithFiltersAsync(Message message, CancellationToken token)
    {
        var state = _states.GetState(message);

        state.Filters.StartDate = DateTime.Now;
        state.StateMachine = _factory.Create(_bot, message.Chat.Id, _grpcService);

        await _factory.FireAsync(new StateMachine<State, Trigger>.TriggerWithParameters<Message>(Trigger.Start), message, state.StateMachine);

        _states.Add(message.Chat.Username, state);
    }

    private async Task HandleStartAsync(Message message, CancellationToken token)
    {
        await _bot.SendStickerAsync
        (
            chatId: message.Chat.Id,
            InputFile.FromFileId(Stickers.Start),
            cancellationToken: token
        );

        await _bot.SendTextMessageAsync
        (
            chatId: message.Chat.Id,
            text: Printer.GetStartText(message),
            parseMode: ParseMode.Html,
            replyToMessageId: message.MessageId,
            cancellationToken: token
        );
    }

    private async Task HandleDevsAsync(Message message, CancellationToken token)
    {
        await _bot.SendTextMessageAsync
        (
            chatId: message.Chat.Id,
            text: Printer.GetDevsText(),
            parseMode: ParseMode.Html,
            replyToMessageId: message.MessageId,
            disableWebPagePreview: true,
            cancellationToken: token
        );

        await _bot.SendStickerAsync
        (
            chatId: message.Chat.Id,
            InputFile.FromFileId(Stickers.Devs),
            cancellationToken: token
        );
    }

    private async Task HandleTopBondsAsync(Message message, CancellationToken token)
    {
        await _bot.SendStickerAsync
        (
            message.Chat.Id,
            InputFile.FromFileId(Stickers.StartOfBonds),
            cancellationToken: token
        );

        await _bot.SendTextMessageAsync
        (
            message.Chat.Id,
            "Выберите действие:",
            replyToMessageId: message.MessageId,
            replyMarkup: new InlineKeyboardMarkup(new InlineKeyboardButton[][]
            {
                [InlineKeyboardButton.WithCallbackData("С фильтрами", "/TOP_BONDS_WITH_FILTERS")],
                [InlineKeyboardButton.WithCallbackData("Без фильтров", "/TOP_BONDS_NO_FILTERS")]
            }),
            cancellationToken: token
        );
    }

    private async Task DoNothingAsync(Message message, CancellationToken token)
    {
        await _bot.SendStickerAsync
        (
            message.Chat.Id,
            InputFile.FromFileId(Stickers.DoNothing),
            cancellationToken: token
        );

        await _bot.SendTextMessageAsync
        (
            message.Chat.Id,
            "Кажется, я не знаю что это значит",
            replyToMessageId: message.MessageId,
            cancellationToken: token
        );
    }

    private async Task ErrorAsync(ITelegramBotClient bot, Exception exception, CancellationToken token)
    {
    }
}