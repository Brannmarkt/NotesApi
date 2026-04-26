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

        // 1. Визначаємо статус-код та повідомлення залежно від типу помилки
        var (statusCode, message, errors) = exception switch
        {
            // Якщо помилка прийшла від FluentValidation
            ValidationException valEx => (
                (int)HttpStatusCode.BadRequest,
                "Validation Failed",
                valEx.Errors.Select(e => new { Field = e.PropertyName, Error = e.ErrorMessage })
            ),

            // Всі інші непередбачені помилки
            _ => (
                (int)HttpStatusCode.InternalServerError,
                "Internal Server Error. Please try again later.",
                null
            )
        };

        context.Response.StatusCode = statusCode;

        // 2. Формуємо об'єкт відповіді
        var response = new
        {
            StatusCode = statusCode,
            Message = message,
            Errors = errors, // Список конкретних полів (тільки для валідації)
            Trace = env.IsDevelopment() ? exception.StackTrace : null
        };

        // 3. Записуємо JSON безпосередньо у відповідь
        await context.Response.WriteAsJsonAsync(response);
    }
}
