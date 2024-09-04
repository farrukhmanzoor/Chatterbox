﻿
using System.Net;
using System.Text.Json;

namespace Chatterbox.Web
{
    public class GlobalExceptionHandlingMiddleware : IMiddleware
    {
        public readonly ILogger<GlobalExceptionHandlingMiddleware> Logger;

        public GlobalExceptionHandlingMiddleware(ILogger<GlobalExceptionHandlingMiddleware> Logger)
        {
            this.Logger = Logger;
        }
        public async Task InvokeAsync(HttpContext context, RequestDelegate next)
        {
            try
            {
                await next(context);
            }
            catch (Exception exception)
            {
                await HandleExceptionAsync(context, exception);
            }
        }

        private async Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            Logger.LogError(exception, exception.Message, exception.StackTrace);
            HttpStatusCode statusCode;
            string massege = exception.Message;

            var exceptionType = exception.GetType();

            if (exceptionType == typeof(MissingMethodException))
            {
                statusCode = HttpStatusCode.NotFound;
            }
            else if (exceptionType == typeof(BadHttpRequestException))
            {
                statusCode = HttpStatusCode.BadRequest;
            }
            else if (exceptionType == typeof(UnauthorizedAccessException))
            {
                statusCode = HttpStatusCode.Unauthorized;
                massege = "Unothorized";
            }
            else
            {
                statusCode = HttpStatusCode.InternalServerError;
            }

            var exceptionResult = JsonSerializer.Serialize(new { error = massege });
            context.Response.ContentType = "application/json";
            context.Response.StatusCode = (int)statusCode;

            await context.Response.WriteAsync(exceptionResult);

        }
    }
}

