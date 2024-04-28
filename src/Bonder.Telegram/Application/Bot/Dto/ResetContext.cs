using Telegram.Bot.Types;

namespace Application.Bot.Dto;

public readonly record struct ResetContext(Message Message, Exception Exception);