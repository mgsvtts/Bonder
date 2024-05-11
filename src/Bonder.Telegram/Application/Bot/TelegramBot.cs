using Application.Bot.Dto;
using Application.Bot.Receivers;
using Application.Helpers;
using Bonder.Calculation.Grpc;
using Bonder.Portfolio.Grpc;
using Mapster;
using Stateless;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;

namespace Application.Bot;

public sealed class TelegramBot
{
    private static readonly StateDictionary _states = new();
    private static readonly StateMachineFactory _factory = new(_states);

    private readonly MessageReceiver _messageReceiver;
    private readonly CallbackReceiver _callbackReceiver;

    public TelegramBot(ITelegramBotClient bot, CalculationService.CalculationServiceClient calculationGrpcClient, PortfolioService.PortfolioServiceClient portfolioGrpcClient)
    {
        _callbackReceiver = new CallbackReceiver(_states, _factory, bot, calculationGrpcClient);
        _messageReceiver = new MessageReceiver(_states, bot, _factory, portfolioGrpcClient);
    }

    public async Task HandleUpdateAsync(Update update, CancellationToken token)
    {
        await (update switch
        {
            { Message: { } message } => _messageReceiver.ReceiveMessage(message, token),
            { CallbackQuery: { } callbackQuery } => _callbackReceiver.ReceivedCallback(callbackQuery, token),
            _ => Task.CompletedTask
        });
    }

    private async Task ErrorAsync(ITelegramBotClient bot, Exception exception, CancellationToken token)
    {
    }
}