using System.Text.RegularExpressions;
using Dapper;

namespace ECAbogados.Infrastructure.Persistence;

public static partial class DatabaseInitializer
{
    public static async Task InitializeAsync(SqlConnectionFactory connectionFactory, string schemaPath)
    {
        if (!File.Exists(schemaPath))
            throw new FileNotFoundException("No se encontró el esquema SQL de inicialización.", schemaPath);

        var script = await File.ReadAllTextAsync(schemaPath);
        var batches = GoSeparator().Split(script)
            .Where(batch => !string.IsNullOrWhiteSpace(batch));

        using var connection = await connectionFactory.CreateOpenConnectionAsync();
        foreach (var batch in batches)
            await connection.ExecuteAsync(batch, commandTimeout: 120);
    }

    [GeneratedRegex(@"^\s*GO\s*$", RegexOptions.Multiline | RegexOptions.IgnoreCase)]
    private static partial Regex GoSeparator();
}
