using Microsoft.Extensions.Logging;

namespace DotaData.Logging;

internal static class LoggerExtensions
{
    public static void LogApiError(this ILogger logger, Exception error)
    {
        logger.LogError("Error when calling API: {error}", error);
    }
}