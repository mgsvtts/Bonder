using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Domain.Common.Exceptions;
using Grpc.Core;

namespace Presentation.Middlewares;
public sealed class CustomExceptionFilter : IExceptionFilter
{
    public void OnException(ExceptionContext context)
    {
        var actionName = context.ActionDescriptor.DisplayName;
        var exceptionStack = context.Exception.StackTrace;
        var exceptionMessage = context.Exception.Message;

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
                Content = $"Error in {actionName}: \n {exceptionMessage} \n {exceptionStack}",
                StatusCode = 500,
                ContentType = "text/plain",
            },
        };
    }
}