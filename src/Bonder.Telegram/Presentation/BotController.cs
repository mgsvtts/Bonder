using Application.Bot;
using Microsoft.AspNetCore.Mvc;
using Presentation.Filters;
using Telegram.Bot.Types;

namespace Presentation;

public sealed class BotController : ControllerBase
{
    private readonly TelegramBot _bot;

    public BotController(TelegramBot bot)
    {
        _bot = bot;
    }

    [HttpPost]
    [ValidateTelegramBot]
    public async Task<IActionResult> Post([FromBody] Update update, CancellationToken cancellationToken)
    {
        await _bot.HandleUpdateAsync(update, cancellationToken);

        return Ok();
    }
}