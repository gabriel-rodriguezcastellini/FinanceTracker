using Microsoft.Extensions.Logging;

namespace FinanceTracker.Core
{
    public class ExceptionLogger(ILogger<ExceptionLogger> logger)
    {
        public void LogException(Exception ex)
        {
            logger.LogError(ex, "An unhandled exception occurred");
        }
    }
}
