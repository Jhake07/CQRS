using CQRS.Application.Contracts.Logging;
using Microsoft.Extensions.Logging;

namespace CQRS.Infrastructure.Logging
{
    public class LoggerAdapter<T>(ILoggerFactory loggerFactory) : IAppLogger<T>
    {
        private readonly ILogger _logger = loggerFactory.CreateLogger<T>();

        public void LogInformation(string message, params object[] args)
        {
            _logger.LogInformation(message, args);
        }

        public void LogWarning(string message, params object[] args)
        {
            _logger?.LogWarning(message, args);
        }
    }
}
