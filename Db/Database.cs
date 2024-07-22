﻿using DbUp;
using DotaData.Configuration;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Options;

namespace DotaData.Db;

internal class Database(IOptions<DbSettings> settings, DbUpgradeLogger logger)
{
    public SqlConnection CreateConnection()
    {
        var connection = new SqlConnection(settings.Value.ConnectionString);
        connection.Open();

        return connection;
    }

    public void Init()
    {
        EnsureDatabase.For.SqlDatabase(settings.Value.ConnectionString);

        var upgrader = DeployChanges.To.SqlDatabase(settings.Value.ConnectionString)
            .WithScriptsEmbeddedInAssembly(typeof(Database).Assembly)
            .LogTo(logger)
            .Build();

        if (!upgrader.IsUpgradeRequired()) return;

        var result = upgrader.PerformUpgrade();
        if (!result.Successful)
            throw new InvalidOperationException("Unable to initialise database", result.Error);
    }
}