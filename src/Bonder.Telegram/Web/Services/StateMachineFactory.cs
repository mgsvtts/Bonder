using Bonder.Calculation.Grpc;
using Mapster;
using Stateless;
using System.Collections.Concurrent;
using System.ComponentModel.DataAnnotations;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;
using Web.Services.Dto;

namespace Web.Services;

public sealed class StateMachineFactory
{
    private readonly StateDictionary _states;
    private CalculationService.CalculationServiceClient _grpcClient;

    public StateMachineFactory(StateDictionary states)
    {
        _states = states;
    }

    public StateMachine<State, Trigger> Create(ITelegramBotClient bot, long chatId, CalculationService.CalculationServiceClient grpcClient)
    {
        var machine = new StateMachine<State, Trigger>(State.Starting);

        var nextTrigger = machine.SetTriggerParameters<Message>(Trigger.Next);
        var skipTrigger = machine.SetTriggerParameters<Message>(Trigger.Skip);
        var resetTrigger = machine.SetTriggerParameters<ResetContext>(Trigger.Reset);
        var startTrigger = machine.SetTriggerParameters<Message>(Trigger.Start);

        machine.Configure(State.Starting)
            .Permit(Trigger.Next, State.GettingPriceFrom)
            .Permit(Trigger.Skip, State.GettingPriceFrom)
            .PermitReentry(Trigger.Start)
            .OnEntryFromAsync(startTrigger, async message =>
            {
                await StartAsync(bot, message);
                await PrintEnterPriceFrom(bot, chatId);
            })
            .OnEntryFromAsync(resetTrigger, context => ResetAsync(bot, context))
            .OnEntryFromAsync(skipTrigger, context => SkipAsync(bot, context));

        machine.Configure(State.GettingPriceFrom)
             .Permit(Trigger.Next, State.GettingPriceTo)
             .Permit(Trigger.Reset, State.Starting)
             .Permit(Trigger.Skip, State.GettingPriceTo)
             .OnEntryFromAsync(nextTrigger, message => GetPriceFromAsync(bot, message))
             .OnEntryFromAsync(skipTrigger, context => SkipAsync(bot, context))
             .OnEntryAsync(x => PrintEnterPriceTo(bot, chatId));

        machine.Configure(State.GettingPriceTo)
             .Permit(Trigger.Next, State.GettingRatingFrom)
             .Permit(Trigger.Reset, State.Starting)
             .Permit(Trigger.Skip, State.GettingRatingFrom)
             .OnEntryFromAsync(nextTrigger, message => GetPriceToAsync(bot, message))
             .OnEntryFromAsync(skipTrigger, context => SkipAsync(bot, context))
             .OnEntryAsync(x => PrintEnterRatingFrom(bot, chatId));

        machine.Configure(State.GettingRatingFrom)
             .Permit(Trigger.Next, State.GettingRatingTo)
             .Permit(Trigger.Reset, State.Starting)
             .Permit(Trigger.Skip, State.GettingRatingTo)
             .OnEntryFromAsync(nextTrigger, message => GetRatingFromAsync(bot, message))
             .OnEntryFromAsync(skipTrigger, context => SkipAsync(bot, context))
             .OnEntryAsync(x => PrintEnterRatingTo(bot, chatId));

        machine.Configure(State.GettingRatingTo)
             .Permit(Trigger.Next, State.GettingUnknownRatings)
             .Permit(Trigger.Reset, State.Starting)
             .Permit(Trigger.Skip, State.GettingUnknownRatings)
             .OnEntryFromAsync(nextTrigger, message => GetRatingToAsync(bot, message))
             .OnEntryFromAsync(skipTrigger, context => SkipAsync(bot, context))
             .OnEntryAsync(x => PrintEnterUnknownRatings(bot, chatId));

        machine.Configure(State.GettingUnknownRatings)
             .Permit(Trigger.Next, State.GettingDateFrom)
             .Permit(Trigger.Reset, State.Starting)
             .Permit(Trigger.Skip, State.GettingDateFrom)
             .OnEntryFromAsync(nextTrigger, message => GetUnknownRatingsAsync(bot, message))
             .OnEntryFromAsync(skipTrigger, context => SkipAsync(bot, context))
             .OnEntryAsync( x => PrintEnterDateFrom(bot, chatId));

        machine.Configure(State.GettingDateFrom)
             .Permit(Trigger.Next, State.GettingDateTo)
             .Permit(Trigger.Reset, State.Starting)
             .Permit(Trigger.Skip, State.GettingDateTo)
             .OnEntryFromAsync(nextTrigger, message => GetDateFromAsync(bot, message))
             .OnEntryFromAsync(skipTrigger, context => SkipAsync(bot, context))
             .OnEntryAsync(x => PrintEnterDateTo(bot, chatId));

        machine.Configure(State.GettingDateTo)
             .Permit(Trigger.Next, State.Finished)
             .Permit(Trigger.Reset, State.Starting)
             .Permit(Trigger.Skip, State.Finished)
             .OnEntryFromAsync(nextTrigger, message => GetDateToAsync(bot, message))
             .OnEntryFromAsync(skipTrigger, context => SkipAsync(bot, context))
             .InitialTransition(State.Finished);

        machine.Configure(State.Finished)
             .Permit(Trigger.Reset, State.Starting)
             .OnEntryFromAsync(skipTrigger, message => FinishAsync(bot, message))
             .OnEntryFromAsync(nextTrigger, message => FinishAsync(bot, message))
             .OnEntryFromAsync(resetTrigger, context => ResetAsync(bot, context))
             .SubstateOf(State.GettingDateTo);

        _grpcClient = grpcClient;

        return machine;
    }

    public async Task FireAsync(StateMachine<State,Trigger>.TriggerWithParameters<Message> trigger, Message message, StateMachine<State, Trigger> machine)
    {
        try
        {
            await machine.FireAsync(trigger, message);
        }
        catch(Exception ex)
        {
            await machine.FireAsync(new StateMachine<State, Trigger>.TriggerWithParameters<ResetContext>(Trigger.Reset), new ResetContext(message, ex));
        }
    }

    public async Task FireSkipAsync(Message message, StateMachine<State, Trigger> machine)
    {
        await machine.FireAsync(new StateMachine<State, Trigger>.TriggerWithParameters<Message>(Trigger.Skip), message);
    }

    private static async Task StartAsync(ITelegramBotClient bot, Message message)
    {
        await bot.SendTextMessageAsync
        (
            message.Chat.Id,
            $"/skip - пропустить фильтр (будет установлено значение по умолчанию)\n" +
            $"/exit - выйти из режима фильтрации"
        );
    }

    private async Task GetPriceFromAsync(ITelegramBotClient bot, Message message)
    {
        var state = _states.GetState(message);

        state.Filters.PriceFrom = decimal.Parse(message.Text);

        if (state.Filters.PriceFrom < 0)
        {
            throw new ValidationException("\"Цена от\" не может быть меньше 0");
        }
    }

    private async Task GetPriceToAsync(ITelegramBotClient bot, Message message)
    {
        var state = _states.GetState(message);

        state.Filters.PriceTo = decimal.Parse(message.Text);

        if (state.Filters.PriceTo < 0)
        {
            throw new ValidationException("\"Цена до\" не может быть меньше 0");
        }
    }

    private async Task GetRatingFromAsync(ITelegramBotClient bot, Message message)
    {
        var state = _states.GetState(message);

        state.Filters.RatingFrom = int.Parse(message.Text);

        if (state.Filters.RatingFrom <= 0 || state.Filters.RatingFrom > 10)
        {
            throw new ValidationException("\"Рейтинг от\" не может быть меньше 0 или больше 10");
        }
    }

    private async Task GetRatingToAsync(ITelegramBotClient bot, Message message)
    {
        var state = _states.GetState(message);

        state.Filters.RatingTo = int.Parse(message.Text);

        if (state.Filters.RatingTo <= 0 || state.Filters.RatingTo > 10)
        {
            throw new ValidationException("\"Рейтинг до\" не может быть меньше 0 или больше 10");
        }
    }

    private async Task GetUnknownRatingsAsync(ITelegramBotClient bot, Message message)
    {
        var state = _states.GetState(message);

        bool value;
        if (message.Text.Contains("ДА", StringComparison.CurrentCultureIgnoreCase))
        {
            value = true;
        }
        else if (message.Text.Contains("НЕТ", StringComparison.CurrentCultureIgnoreCase))
        {
            value = false;
        }
        else
        {
            throw new ValidationException("Сообщение не содержит слова \"да\" или \"нет\"");
        }

        state.Filters.IncludeUnknownRatings = value;
    }


    private async Task GetDateFromAsync(ITelegramBotClient bot, Message message)
    {
        var state = _states.GetState(message);

        if (message.Text.Contains("СЕЙЧАС", StringComparison.CurrentCultureIgnoreCase))
        {
            state.Filters.DateFrom = DateOnly.FromDateTime(DateTime.Now);
        }
        else
        {
            state.Filters.DateFrom = DateOnly.ParseExact(message.Text, "d/M/yyyy");
        }
    }

    private async Task GetDateToAsync(ITelegramBotClient bot, Message message)
    {
        var state = _states.GetState(message);

        state.Filters.DateTo = DateOnly.ParseExact(message.Text, "d/M/yyyy");

        if (state.Filters.DateTo <= state.Filters.DateFrom)
        {
            throw new ValidationException("\"Дата от\" не может быть больше или равна \"Дате до\"");
        }

        if(state.Filters.DateTo < DateOnly.FromDateTime(DateTime.Now))
        {
            throw new ValidationException("\"Дата от\" не может быть в прошлом");
        }
    }

    private async Task FinishAsync(ITelegramBotClient bot, Message message)
    {
        var state = _states.GetState(message);

        await bot.SendTextMessageAsync
        (
            message.Chat.Id,
            Printer.GetBondsFinish(state.Filters),
            parseMode: ParseMode.Html
        );

        await bot.SendChatActionAsync
        (
            message.Chat.Id,
            ChatAction.Typing
        );

        var bonds = await _grpcClient.GetCurrentBondsAsync(state.Filters.Adapt<Filters>());

        await bot.SendTextMessageAsync
        (
            message.Chat.Id,
            Printer.GetTopBondsText(bonds.Bonds),
            parseMode: ParseMode.Html
        );

        state.Filters = BondFilters.Default;
    }

    private async Task ResetAsync(ITelegramBotClient bot, ResetContext context)
    {
        var state = _states.GetState(context.Message);

        state.Filters = new BondFilters
        {
            StartDate = state.Filters.StartDate
        };

        await bot.SendTextMessageAsync
        (
            context.Message.Chat.Id,
            "Произошла ошибка ввода, состояние фильтров сброшено\n/exit - выйти из режима фильтрации"
        );

        if(context.Exception is ValidationException)
        {
            await bot.SendTextMessageAsync
            (
                context.Message.Chat.Id,
                $"<b>ОШИБКА: {context.Exception.Message}</b>",
                parseMode: ParseMode.Html
            );
        }

        await bot.SendStickerAsync(context.Message.Chat.Id, InputFile.FromFileId(Stickers.FiltersError));

        await FireAsync(new StateMachine<State, Trigger>.TriggerWithParameters<Message>(Trigger.Start), context.Message, state.StateMachine);
    }

    private async Task SkipAsync(ITelegramBotClient bot, Message message)
    {
        var state = _states.GetState(message);
        var filters = state.Filters;
        var machine = state.StateMachine;

        string setted = "";
        if (machine.State == State.GettingPriceFrom)
        {
            filters.PriceFrom = 0;
            setted = "0";
        }
        else if (machine.State == State.GettingPriceTo)
        {
            filters.PriceTo = int.MaxValue;
            setted = "Очень большое число";
        }
        else if (machine.State == State.GettingRatingFrom)
        {
            filters.RatingFrom = 0; 
            setted = "0";
        }
        else if (machine.State == State.GettingRatingTo)
        {
            filters.RatingTo = 10;
            setted = "10";
        }
        else if (machine.State == State.GettingUnknownRatings)
        {
            filters.IncludeUnknownRatings = true;
            setted = "Да";
        }
        else if (machine.State == State.GettingDateFrom)
        {
            filters.DateFrom = DateOnly.FromDateTime(DateTime.Now);
            setted = filters.DateFrom.ToString();
        }
        else if (machine.State == State.GettingDateTo)
        {
            filters.DateTo = DateOnly.MaxValue;
            setted = "Начало времён";
        }

        await bot.SendTextMessageAsync
        (
            message.Chat.Id,
            $"Фильтр пропущен, установлено значение: <b>{setted}</b>",
            replyToMessageId:message.MessageId,
            parseMode: ParseMode.Html
        );
    }
    private static async Task PrintEnterPriceFrom(ITelegramBotClient bot, long chatId)
    {
        await bot.SendTextMessageAsync
        (
            chatId,
            "Введите \"Цену от\":"
        );
    }
    private static async Task PrintEnterPriceTo(ITelegramBotClient bot, long chatId)
    {
        await bot.SendTextMessageAsync
        (
            chatId,
            "Введите \"Цену до\":"
        );
    }
    private static async Task PrintEnterRatingFrom(ITelegramBotClient bot, long chatId)
    {
        await bot.SendTextMessageAsync
        (
            chatId,
            "Введите \"Рейтинг от\" (от 1 до 10 включительно):"
        );
    }
    private static async Task PrintEnterRatingTo(ITelegramBotClient bot, long chatId)
    {
        await bot.SendTextMessageAsync
        (
            chatId,
            "Введите \"Рейтинг до\" (от 1 до 10 включительно):"
        );
    }
    private static async Task PrintEnterUnknownRatings(ITelegramBotClient bot, long chatId)
    {
        var replyMarkup = new InlineKeyboardMarkup(new InlineKeyboardButton[][]
        {
            [InlineKeyboardButton.WithCallbackData("Да", "/BONDS_WITH_UNKNOWN_RATINGS")],
            [InlineKeyboardButton.WithCallbackData("Нет", "/BONDS_WITHOUT_UNKNOWN_RATINGS")]
        });

        await bot.SendTextMessageAsync
        (
            chatId,
            "Включать ли эмитентов с неизвестным рейтингом (да|нет)?",
            replyMarkup: replyMarkup
        );
    }

    private static async Task PrintEnterDateFrom(ITelegramBotClient bot, long chatId)
    {
        await bot.SendTextMessageAsync
        (
            chatId,
            "Введите \"Дата от\" (в формате \"20/12/2023\" или напишите \"сейчас\" чтобы взять сегодняшную дату):"
        );
    }

    private static async Task PrintEnterDateTo(ITelegramBotClient bot, long chatId)
    {
        await bot.SendTextMessageAsync
        (
            chatId,
            "Введите \"Дата до\" (в формате \"20/12/2023\":"
        );
    }

}
