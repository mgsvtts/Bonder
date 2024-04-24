using Stateless;
using System.Collections.Concurrent;
using Telegram.Bot;
using Telegram.Bot.Types;
using Web.Services.Dto;

namespace Web.Services;

public sealed class StateMachineFactory
{
    private readonly StateDictionary _states;
    private StateMachine<State, Trigger> _machine;

    public StateMachineFactory( StateDictionary states)
    {
        _states = states;
    }

    public StateMachine<State, Trigger> Create(ITelegramBotClient bot)
    {
        var machine = new StateMachine<State, Trigger>(State.Starting);

        var nextTrigger = machine.SetTriggerParameters<Message>(Trigger.Next);
        var resetTrigger = machine.SetTriggerParameters<Message>(Trigger.Reset);
        var startTrigger = machine.SetTriggerParameters<Message>(Trigger.Start);

        machine.Configure(State.Starting)
            .Permit(Trigger.Next, State.GettingPriceFrom)
            .PermitReentry(Trigger.Start)
            .OnEntryFromAsync(startTrigger, message => StartAsync(bot, message))
            .OnEntryFromAsync(resetTrigger, message => ResetAsync(bot, message));

        machine.Configure(State.GettingPriceFrom)
            .Permit(Trigger.Next, State.GettingPriceTo)
            .Permit(Trigger.Reset, State.Starting)
            .OnEntryFromAsync(nextTrigger, message => GetPriceFromAsync(bot, message));

        machine.Configure(State.GettingPriceTo)
            .Permit(Trigger.Next, State.GettingRatingFrom)
            .Permit(Trigger.Reset, State.Starting)
            .OnEntryFromAsync(nextTrigger, message => GetPriceToAsync(bot, message));

        _machine = machine;

        return machine;
    }

    public async Task FireAsync(StateMachine<State,Trigger>.TriggerWithParameters<Message> trigger, Message message)
    {
        try
        {
            await _machine.FireAsync(trigger, message);
        }
        catch
        {
            await _machine.FireAsync(new StateMachine<State, Trigger>.TriggerWithParameters<Message>(Trigger.Reset), message);
        }
    }

    private static async Task StartAsync(ITelegramBotClient bot, Message message)
    {
        await bot.SendTextMessageAsync
        (
            message.Chat.Id,
            $"Введите /skip чтобы пропустить фильтр (будет установлено значение по умолчанию)"
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
        var value = decimal.Parse(message.Text);

        state.Filters.PriceFrom = value;

        await bot.SendTextMessageAsync
        (
            message.Chat.Id,
            "Введите \"Цену до\":"
       );
    }

    private async Task GetPriceToAsync(ITelegramBotClient bot, Message message)
    {
        var state = _states.GetState(message);
        var value = decimal.Parse(message.Text);

        state.Filters.PriceTo = value;

        await bot.SendTextMessageAsync
        (
            message.Chat.Id,
            "Введите \"Рейтинг от\":"
        );
    }

    private async Task GetRatingFromAsync(ITelegramBotClient bot, Message message)
    {
        var state = _states.GetState(message);
        var value = decimal.Parse(message.Text);

        state.Filters.RatingFrom = value;

        await bot.SendTextMessageAsync
        (
            message.Chat.Id,
            "Ок"
        );
    }

    private async Task ResetAsync(ITelegramBotClient bot, Message message)
    {
        var state = _states.GetState(message);

        state.Filters = new BondFilters();

        await bot.SendTextMessageAsync
        (
            message.Chat.Id,
            "Произошла ошибка ввода, состояние фильтров сброшено"
        );

        await bot.SendStickerAsync(message.Chat.Id, InputFile.FromFileId(Stickers.FiltersError));

        await FireAsync(new StateMachine<State, Trigger>.TriggerWithParameters<Message>(Trigger.Start), message);
    }
}
