using DbUp;
using DotaData.Configuration;
using Microsoft.Extensions.Options;
using Npgsql;

namespace DotaData.Db;

internal class Database(IOptions<DbSettings> settings, DbUpgradeLogger logger)
{
    public NpgsqlConnection CreateConnection()
    {
        var connection = new NpgsqlConnection(settings.Value.ConnectionString);
        connection.Open();

        return connection;
    }

    public void Init()
    {
        EnsureDatabase.For.PostgresqlDatabase(settings.Value.ConnectionString);

        var upgrader = DeployChanges.To.PostgresqlDatabase(settings.Value.ConnectionString)
            .WithScriptsEmbeddedInAssembly(typeof(Database).Assembly)
            .LogTo(logger)
            .Build();

        if (!upgrader.IsUpgradeRequired()) return;

        var result = upgrader.PerformUpgrade();
        if (!result.Successful)
            throw new InvalidOperationException("Unable to initialise database", result.Error);
    }
}