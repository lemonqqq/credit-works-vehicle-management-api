using CreditWorks.VehicleManagement.Application.Contracts;
using Microsoft.Extensions.Logging;

namespace CreditWorks.VehicleManagement.Infrastructure.Common
{
    public class LoggingService<TClass>(ILogger<TClass> logger) : ILoggingService<TClass>
        where TClass : class
    {
        public void LogDebug(string messageTemplate, params object?[] args)
        {
            logger.LogDebug(messageTemplate, args);
        }

        public void LogWarning(Exception ex, string messageTemplate, params object?[] args)
        {
            logger.LogWarning(ex, messageTemplate, args);
        }

        public void LogInfo(string messageTemplate, params object?[] args)
        {
            logger.LogInformation(messageTemplate, args);
        }

        public void LogError(Exception ex, string messageTemplate, params object?[] args)
        {
            logger.LogError(ex, messageTemplate, args);
        }

    }
}
