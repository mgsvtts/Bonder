using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;

namespace Presentation;
public sealed class BotController : ControllerBase
{
    [HttpPost]
    [ValidateTelegramBot]
    public async Task<IActionResult> Post(
    [FromBody] Update update,
        [FromServices] UpdateHandlers handleUpdateService,
        CancellationToken cancellationToken)
    {
        await handleUpdateService.HandleUpdateAsync(update, cancellationToken);
        return Ok();
    }
}

[AttributeUsage(AttributeTargets.Method)]
public sealed class ValidateTelegramBotAttribute : TypeFilterAttribute
{
    public ValidateTelegramBotAttribute()
        : base(typeof(ValidateTelegramBotFilter))
    { }

    private class ValidateTelegramBotFilter : IActionFilter
    {
        private readonly string _secretToken;

        public ValidateTelegramBotFilter(IOptions<BotConfiguration> options)
        {
            var botConfiguration = options.Value;
            _secretToken = botConfiguration.SecretToken;
        }

        public void OnActionExecuted(ActionExecutedContext context)
        {
        }

        public void OnActionExecuting(ActionExecutingContext context)
        {
            if (IsValidRequest(context.HttpContext.Request))
            {
                return;
            }

            context.Result = new ObjectResult("\"X-Telegram-Bot-Api-Secret-Token\" is invalid")
            {
                StatusCode = (int)HttpStatusCode.Forbidden
            };
        }

        private bool IsValidRequest(HttpRequest request)
        {
            var isSecretTokenProvided = request.Headers.TryGetValue("X-Telegram-Bot-Api-Secret-Token", out var secretTokenHeader);
            if (!isSecretTokenProvided) return false;

            return string.Equals(secretTokenHeader, _secretToken, StringComparison.Ordinal);
        }
    }
}