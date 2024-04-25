using Bonder.Calculation.Grpc;
using Mapster;
using Stateless;
using System.Collections.Concurrent;
using System.ComponentModel.DataAnnotations;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
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

    public StateMachine<State, Trigger> Create(ITelegramBotClient bot, CalculationService.CalculationServiceClient grpcClient)
    {
        var machine = new StateMachine<State, Trigger>(State.Starting);

        var nextTrigger = machine.SetTriggerParameters<Message>(Trigger.Next);
        var resetTrigger = machine.SetTriggerParameters<ResetContext>(Trigger.Reset);
        var startTrigger = machine.SetTriggerParameters<Message>(Trigger.Start);

        machine.Configure(State.Starting)
            .Permit(Trigger.Next, State.GettingPriceFrom)
            .PermitReentry(Trigger.Start)
            .OnEntryFromAsync(startTrigger, message => StartAsync(bot, message))
            .OnEntryFromAsync(resetTrigger, context => ResetAsync(bot, context));

        machine.Configure(State.GettingPriceFrom)
             .Permit(Trigger.Next, State.GettingPriceTo)
             .Permit(Trigger.Reset, State.Starting)
             .OnEntryFromAsync(nextTrigger, message => GetPriceFromAsync(bot, message));

        machine.Configure(State.GettingPriceTo)
             .Permit(Trigger.Next, State.GettingRatingFrom)
             .Permit(Trigger.Reset, State.Starting)
             .OnEntryFromAsync(nextTrigger, message => GetPriceToAsync(bot, message));

        machine.Configure(State.GettingRatingFrom)
             .Permit(Trigger.Next, State.GettingRatingTo)
             .Permit(Trigger.Reset, State.Starting)
             .OnEntryFromAsync(nextTrigger, message => GetRatingFromAsync(bot, message));

        machine.Configure(State.GettingRatingTo)
             .Permit(Trigger.Next, State.GettingUnknownRatings)
             .Permit(Trigger.Reset, State.Starting)
             .OnEntryFromAsync(nextTrigger, message => GetRatingToAsync(bot, message));

        machine.Configure(State.GettingUnknownRatings)
             .Permit(Trigger.Next, State.GettingDateFrom)
             .Permit(Trigger.Reset, State.Starting)
             .OnEntryFromAsync(nextTrigger, message => GetUnknownRatingsAsync(bot, message));

        machine.Configure(State.GettingDateFrom)
             .Permit(Trigger.Next, State.Finished)
             .Permit(Trigger.Reset, State.Starting)
             .OnEntryFromAsync(nextTrigger, message => GetDateFromAsync(bot, message));

        machine.Configure(State.Finished)
             .Permit(Trigger.Reset, State.Starting)
             .OnEntryFromAsync(nextTrigger, message => GetDateToAsync(bot, message));

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
            await machine.FireAsync(new StateMachine<State, Trigger>.TriggerWithParameters<ResetContext>(Trigger.Reset), new ResetContext(message, machine, ex));
        }
    }

    private static async Task StartAsync(ITelegramBotClient bot, Message message)
    {
        await bot.SendTextMessageAsync
        (
            message.Chat.Id,
            $"/skip - пропустить фильтр (будет установлено значение по умолчанию)\n" +
            $"/exit - выйти из режима фильтрации"
        );

        await bot.SendTextMessageAsync
        (
            message.Chat.Id,
            "Введите \"Цену от\":"
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

        await bot.SendTextMessageAsync
        (
            message.Chat.Id,
            "Введите \"Цену до\":"
       );
    }

    private async Task GetPriceToAsync(ITelegramBotClient bot, Message message)
    {
        var state = _states.GetState(message);

        state.Filters.PriceTo = decimal.Parse(message.Text);

        if (state.Filters.PriceTo < 0)
        {
            throw new ValidationException("\"Цена до\" не может быть меньше 0");
        }

        await bot.SendTextMessageAsync
        (
            message.Chat.Id,
            "Введите \"Рейтинг от\" (от 1 до 10 включительно):"
        );
    }

    private async Task GetRatingFromAsync(ITelegramBotClient bot, Message message)
    {
        var state = _states.GetState(message);

        state.Filters.RatingFrom = int.Parse(message.Text);

        if (state.Filters.RatingFrom <= 0 || state.Filters.RatingFrom > 10)
        {
            throw new ValidationException("\"Рейтинг от\" не может быть меньше 0 или больше 10");
        }

        await bot.SendTextMessageAsync
        (
            message.Chat.Id,
            "Введите \"Рейтинг до\" (от 1 до 10 включительно):"
        );
    }

    private async Task GetRatingToAsync(ITelegramBotClient bot, Message message)
    {
        var state = _states.GetState(message);

        state.Filters.RatingTo = int.Parse(message.Text);

        if (state.Filters.RatingTo <= 0 || state.Filters.RatingTo > 10)
        {
            throw new ValidationException("\"Рейтинг до\" не может быть меньше 0 или больше 10");
        }

        await bot.SendTextMessageAsync
        (
            message.Chat.Id,
            "Включать ли эмитентов с неизвестным рейтингом (да|нет)?"
        );
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

        await bot.SendTextMessageAsync
        (
            message.Chat.Id,
            "Введите \"Дата от\" (в формате \"20/12/2023\" или напишите \"сейчас\" чтобы взять сегодняшную дату):"
        );
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

        await bot.SendTextMessageAsync
        (
            message.Chat.Id,
            "Введите \"Дата до\" (в формате \"20/12/2023\" или напишите \"сейчас\" чтобы взять сегодняшную дату):"
        );
    }

    private async Task GetDateToAsync(ITelegramBotClient bot, Message message)
    {
        var state = _states.GetState(message);

        if (message.Text.Contains("СЕЙЧАС", StringComparison.CurrentCultureIgnoreCase))
        {
            state.Filters.DateTo = DateOnly.FromDateTime(DateTime.Now);
        }
        else
        {
            state.Filters.DateTo = DateOnly.ParseExact(message.Text, "d/M/yyyy");
        }

        if(state.Filters.DateTo <= state.Filters.DateFrom)
        {
            throw new ValidationException("\"Дата от\" не может быть больше или равна \"Дате до\"");
        }

        await FinishAsync(bot, message);
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
            "Произошла ошибка ввода, состояние фильтров сброшено"
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

        await FireAsync(new StateMachine<State, Trigger>.TriggerWithParameters<Message>(Trigger.Start), context.Message, context.Machine);
    }
}
