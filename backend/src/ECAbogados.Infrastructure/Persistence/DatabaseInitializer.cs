using System.Text.RegularExpressions;
using Dapper;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;

namespace ECAbogados.Infrastructure.Persistence;

public static partial class DatabaseInitializer
{
    public static async Task InitializeAsync(IConfiguration configuration, string schemaPath)
    {
        if (!File.Exists(schemaPath))
            throw new FileNotFoundException("No se encontró el esquema SQL de inicialización.", schemaPath);

        var connectionString = configuration.GetConnectionString("Default")
            ?? throw new InvalidOperationException("No se encontró la cadena de conexión 'Default'.");
        var targetBuilder = new SqlConnectionStringBuilder(connectionString);
        var databaseName = targetBuilder.InitialCatalog;
        if (string.IsNullOrWhiteSpace(databaseName))
            throw new InvalidOperationException("La cadena de conexión debe indicar una base de datos.");

        var masterBuilder = new SqlConnectionStringBuilder(connectionString) { InitialCatalog = "master" };
        await using (var master = new SqlConnection(masterBuilder.ConnectionString))
        {
            await master.OpenAsync();
            const string ensureDatabase = """
                IF DB_ID(@DatabaseName) IS NULL
                BEGIN
                    DECLARE @sql NVARCHAR(MAX) = N'CREATE DATABASE ' + QUOTENAME(@DatabaseName);
                    EXEC sp_executesql @sql;
                END
                """;
            await master.ExecuteAsync(ensureDatabase, new { DatabaseName = databaseName }, commandTimeout: 120);
        }

        var script = await File.ReadAllTextAsync(schemaPath);
        var batches = GoSeparator().Split(script)
            .Where(batch => !string.IsNullOrWhiteSpace(batch));

        await using var connection = new SqlConnection(targetBuilder.ConnectionString);
        await connection.OpenAsync();
        foreach (var batch in batches)
            await connection.ExecuteAsync(batch, commandTimeout: 120);
    }

    [GeneratedRegex(@"^\s*GO\s*$", RegexOptions.Multiline | RegexOptions.IgnoreCase)]
    private static partial Regex GoSeparator();
}
