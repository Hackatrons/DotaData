using DbUp.Engine.Output;
using Microsoft.Extensions.Logging;

namespace DotaData.Db;

internal class DbUpgradeLogger(ILogger<DbUpgradeLogger> logger) : IUpgradeLog
{
    public void WriteInformation(string format, params object[] args)
    {
        logger.LogInformation(format, args);
    }

    public void WriteError(string format, params object[] args)
    {
        logger.LogError(format, args);
    }

    public void WriteWarning(string format, params object[] args)
    {
        logger.LogWarning(format, args);
    }
}