using Application.Commands.CheckAccess.Dto;
using Microsoft.AspNetCore.Http;

namespace Presentation.Extensions;
public static class HttpContextExtensions
{
    public static void SetAccessHeaders(this HttpContext context, AccessResult result)
    {
        if (result.UserId is not null)
        {
            context.Response.Headers.Append("X-USER-ID", result.UserId.ToString());
        }

        if (result.TokenExpired)
        {
            context.Response.Headers.Append("TOKEN-EXPIRED", "true");
        }
    }
}
