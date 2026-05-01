using MediatR;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Common.Behaviors;
public class LoggingBehavior<TRequest, TResponse>(ILogger<TRequest> logger)
    : IPipelineBehavior<TRequest, TResponse>
    where TRequest : notnull
{
    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        var requestName = typeof(TRequest).Name;
        var timer = Stopwatch.StartNew();

        logger.LogInformation("🚀 Початок запиту: {Name} {@Request}", requestName, request);

        try
        {
            var response = await next();
            timer.Stop();

            logger.LogInformation("✅ Запит завершено: {Name} за {ElapsedMilliseconds}ms",
                requestName, timer.ElapsedMilliseconds);

            return response;
        }
        catch (Exception ex)
        {
            timer.Stop();
            logger.LogError(ex, "❌ Помилка у запиті {Name} через {ElapsedMilliseconds}ms",
                requestName, timer.ElapsedMilliseconds);
            throw;
        }
    }
}
