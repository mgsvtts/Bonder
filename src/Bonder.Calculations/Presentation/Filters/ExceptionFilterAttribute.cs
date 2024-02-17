using Domain.Common.Exceptions;
using Grpc.Core;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Net;

namespace Presentation.Filters;

[AttributeUsage(AttributeTargets.Class)]
public sealed class ExceptionFilterAttribute : Attribute, IExceptionFilter
{
    public void OnException(ExceptionContext context)
    {
        context.Result = context.Exception switch
        {
            UnauthorizedAccessException => new ContentResult
            {
                StatusCode = (int)HttpStatusCode.Unauthorized,
                Content = context.Exception.Message,
                ContentType = "text/plain",
            },
            WebException => new ContentResult
            {
                StatusCode = (int)HttpStatusCode.InternalServerError,
                Content = "Error in web request: " + (context.Exception as WebException).Response.ResponseUri,
                ContentType = "text/plain",
            },
            RpcException => new ContentResult
            {
                StatusCode = (int)HttpStatusCode.InternalServerError,
                Content = "Error in rpc request: " + context.Exception.Message,
                ContentType = "text/plain",
            },
            DomainLogicException => new ContentResult
            {
                StatusCode = (int)HttpStatusCode.UnprocessableEntity,
                Content = context.Exception.Message,
                ContentType = "text/plain"
            },
            _ => new ContentResult
            {
                Content = $"Error in {context.ActionDescriptor.DisplayName}:{Environment.NewLine}{context.Exception.Message}",
                StatusCode = 500,
                ContentType = "text/plain",
            },
        };
    }
}