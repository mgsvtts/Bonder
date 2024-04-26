using Stateless;
using Telegram.Bot.Types;

namespace Web.Services.Dto;

public readonly record struct ResetContext(Message Message, Exception Exception);