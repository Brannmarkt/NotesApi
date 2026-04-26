using Application.Common;
using FluentValidation;
using System.Net;

namespace WebApi.Middleware;

public class ExceptionMiddleware(RequestDelegate next, ILogger<ExceptionMiddleware> logger, IHostEnvironment env)
{
    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await next(context);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "An unhandled exception occurred: {Message}", ex.Message);
            await HandleExceptionAsync(context, ex);
        }
    }

    private async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        context.Response.ContentType = "application/json";

        var (statusCode, message, errors) = exception switch
        {
            ValidationException valEx => (
                (int)HttpStatusCode.BadRequest,
                "Validation Failed",
                valEx.Errors.Select(e => new { Field = e.PropertyName, Error = e.ErrorMessage })
            ),
            _ => (
                (int)HttpStatusCode.InternalServerError,
                "Internal Server Error. Please try again later.",
                null
            )
        };

        context.Response.StatusCode = statusCode;

        var response = new
        {
            StatusCode = statusCode,
            Message = message,
            Errors = errors,
            // Додаємо Trace ТІЛЬКИ якщо це не помилка валідації і ми в Dev-режимі
            Trace = (exception is not ValidationException && env.IsDevelopment())
                    ? exception.StackTrace
                    : null
        };

        await context.Response.WriteAsJsonAsync(response);
    }
}
