using Application.Bot.Dto;
using Application.Helpers;
using Bonder.Calculation.Grpc;
using Mapster;
using Stateless;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace Application.Bot.Receivers;
public sealed class CallbackReceiver
{
    private readonly StateDictionary _states;
    private readonly ITelegramBotClient _bot;
    private readonly StateMachineFactory _factory;
    private readonly CalculationService.CalculationServiceClient _grpcService;

    public CallbackReceiver(StateDictionary states, StateMachineFactory factory, ITelegramBotClient bot, CalculationService.CalculationServiceClient grpcClient)
    {
        _states = states;
        _factory = factory;
        _bot = bot;
        _grpcService = grpcClient;
    }

    public async Task ReceivedCallback(CallbackQuery query, CancellationToken token)
    {
        var text = query?.Data?.ToUpper();

        if (string.IsNullOrEmpty(text))
        {
            return;
        }

        await (text switch
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
            _ => Task.CompletedTask
        });
    }
    private async Task HandleDateFromAsync(Message message)
    {
        var state = _states.GetState(message);

        if (state.Filters == BondFilters.Default)
        {
            return;
        }

        state.Filters.DateFrom = DateOnly.FromDateTime(DateTime.Now);

        await _factory.FireAsync(new StateMachine<State, Trigger>.TriggerWithParameters<Message>(Trigger.Next), message, state.StateMachine);
    }

    private async Task HandleDateToAsync(Message message, DateToType type)
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

    private async Task HandleBondsWithUnknownRatingsAsync(Message message, bool value)
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

}
