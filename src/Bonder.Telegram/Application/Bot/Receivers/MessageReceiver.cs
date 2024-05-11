using Application.Bot.Dto;
using Application.Helpers;
using Stateless;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;
using Telegram.Bot.Types;
using Telegram.Bot;
using Bonder.Calculation.Grpc;
using Bonder.Portfolio.Grpc;

namespace Application.Bot.Receivers;
public sealed class MessageReceiver
{
    private static readonly char[] _splitters = [',', ' ', '\n'];

    private readonly StateDictionary _states;
    private readonly ITelegramBotClient _bot;
    private readonly StateMachineFactory _factory;
    private readonly PortfolioService.PortfolioServiceClient _grcpClient;

    public MessageReceiver(StateDictionary states, ITelegramBotClient bot, StateMachineFactory factory, PortfolioService.PortfolioServiceClient grpcClient)
    {
        _states = states;
        _bot = bot;
        _factory = factory;
        _grcpClient = grpcClient;
    }

    public async Task ReceiveMessage(Message message, CancellationToken token)
    {
        if (await TryHandleStickerAsync(message, token))
        {
            return;
        }

        if(await TryHandleFiltersAsync(message))
        {
            return;
        }

        await (message?.Text?.ToUpper().Split(_splitters)[0] switch
        {
            "/START" => HandleStartAsync(message, token),
            "/TOP_BONDS" => HandleTopBondsAsync(message, token),
            "/ATTACH_TOKEN" => HandleAttachTokenAsync(message, token),
            "/DEVS" => HandleDevsAsync(message, token),
            _ => GetInputAsync(message, token)
        });
    }

    private async Task<bool> TryHandleFiltersAsync(Message message)
    {
        var state = _states.GetState(message);
        if (state.Filters == BondFilters.Default)
        {
            return false;
        }

        await HandleFiltersAsync(message, state);

        return true;
    }

    private async Task<bool> TryHandleStickerAsync(Message message, CancellationToken token)
    {
        var sticker = message?.Sticker?.FileId;

        if (string.IsNullOrEmpty(sticker))
        {
            return false;
        }

        await _bot.SendStickerAsync
        (
            message.Chat.Id,
            InputFile.FromFileId(sticker),
            replyToMessageId: message.MessageId,
            cancellationToken: token);

        return true;
    }

    private async Task HandleAttachTokenAsync(Message message, CancellationToken token)
    {
        await _bot.SendTextMessageAsync
        (
            message.Chat.Id,
            Printer.GetAttachToken(),
            parseMode: ParseMode.Html,
            cancellationToken: token
        );
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

    private async Task GetInputAsync(Message message, CancellationToken token)
    {
        if (message.Text.Contains("ТОКЕН:", StringComparison.CurrentCultureIgnoreCase))
        {
            await HandleAttachToken(message, token);
            return;
        }

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

    private async Task HandleAttachToken(Message message, CancellationToken token)
    {
        var tinkoffToken = message.Text.Split(":", StringSplitOptions.RemoveEmptyEntries).Last();

        if (string.IsNullOrEmpty(tinkoffToken))
        {
            await _bot.SendTextMessageAsync
            (
                message.Chat.Id,
                "Ошибка ввода токена",
                cancellationToken: token
            );

            return;
        }

        await _bot.SendChatActionAsync
        (
            message.Chat.Id,
            ChatAction.Typing,
            cancellationToken: token
        );

        await _grcpClient.RefreshTokenAsync(new RefreshTokenRequest
        {
            UserId = Guid.NewGuid(),
            Token = tinkoffToken,
        }, cancellationToken: token);

        await _bot.SendTextMessageAsync
        (
            message.Chat.Id,
            "Токен успено привязан",
            replyToMessageId: message.MessageId,
            cancellationToken: token
        );

        await _bot.SendStickerAsync
        (
            message.Chat.Id,
            InputFile.FromFileId(Stickers.AttachToken),
            cancellationToken: token
        );

        return;
    }
}
