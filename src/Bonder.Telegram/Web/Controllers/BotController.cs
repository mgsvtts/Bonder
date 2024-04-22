using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;
using Web.FilterAttributes;
using Web.Services;

namespace Web.Controllers;

public class BotController : ControllerBase
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