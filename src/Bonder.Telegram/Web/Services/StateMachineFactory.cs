using Stateless;
using System.Collections.Concurrent;
using Telegram.Bot;
using Telegram.Bot.Types;
using Web.Services.Dto;

namespace Web.Services;

public sealed class StateMachineFactory
{
    private readonly StateDictionary _states;

    public StateMachineFactory( StateDictionary states)
    {
        _states = states;
    }

    public StateMachine<State, Trigger> Create(ITelegramBotClient bot, Message message)
    {
        var machine = new StateMachine<State, Trigger>(State.Start);

        machine.Configure(State.Start)
            .Permit(Trigger.GetPriceFrom, State.GettingPriceFrom);

        machine.Configure(State.GettingPriceFrom)
            .OnEntryAsync(x => GetPriceFrom(bot, message))
            .Permit(Trigger.GetPriceTo, State.GettingPriceTo);

        machine.Configure(State.GettingPriceTo)
            .OnEntryAsync(x => GetPriceTo(bot, message))
            .Permit(Trigger.GetRatingFrom, State.GettingRatingFrom);

        return machine;
    }

    private async Task GetPriceFrom(ITelegramBotClient bot, Message message)
    {
        var state = _states.GetState(message);
        var (parsed, value) = await ParseAsync(message, bot);

        if (!parsed)
        {
            return;
        }

        state.Filters.PriceFrom = value;

        await bot.SendTextMessageAsync
        (
            message.Chat.Id,
            "Введите \"Цену до\":"
       );
    }

    private async Task GetPriceTo(ITelegramBotClient bot, Message message)
    {
        var state = _states.GetState(message);
        var (parsed, value) = await ParseAsync(message, bot);

        if (!parsed)
        {
            return;
        }

        state.Filters.PriceTo = value;

        await bot.SendTextMessageAsync
        (
            message.Chat.Id,
            "Введите \"Рейтинг от\":"
        );
    }

    private async Task GetRatingFrom(ITelegramBotClient bot, Message message)
    {
        var state = _states.GetState(message);
        var (parsed, value) = await ParseAsync(message, bot);

        if (!parsed)
        {
            return;
        }

        state.Filters.RatingFrom = value;

        await bot.SendTextMessageAsync
        (
            message.Chat.Id,
            "Ок"
        );
    }

    private static async Task<(bool Parsed, decimal Value)> ParseAsync(Message message, ITelegramBotClient bot)
    {
        if (!decimal.TryParse(message.Text, out var value))
        {
            await bot.SendTextMessageAsync
            (
                message.Chat.Id,
                "Неправильный формат ввода"
            );

            return (false, default!);
        }

        return (true, value);
    }
}
