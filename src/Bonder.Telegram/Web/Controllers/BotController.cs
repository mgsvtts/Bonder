using Microsoft.AspNetCore.Mvc;
using Telegram.Bot.Types;
using Web.FilterAttributes;
using Web.Services;

namespace Web.Controllers;

public sealed class BotController : ControllerBase
{
    private readonly Bot _bot;

    public BotController(Bot bot)
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