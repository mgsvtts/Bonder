using Domain.Exceptions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Net;
using System.Text.Json;

namespace Presentation.Filters;

[AttributeUsage(AttributeTargets.Class)]
public sealed class ExceptionFilterAttribute : Attribute, IExceptionFilter
{
    public void OnException(ExceptionContext context)
    {
        var errors = context.Exception.Message.Split(Environment.NewLine, StringSplitOptions.RemoveEmptyEntries)
        .Select(x => new ErrorMessage(x));

        context.Result = context.Exception switch
        {
            AuthorizationException or UnauthorizedAccessException => new ContentResult
            {
                StatusCode = (int)HttpStatusCode.Unauthorized,
                Content = JsonSerializer.Serialize(errors),
                ContentType = "application/json",
            },
            _ => new ContentResult
            {
                StatusCode = (int)HttpStatusCode.InternalServerError,
                Content = JsonSerializer.Serialize(errors),
                ContentType = "application/json",
            },
        };
    }
}
