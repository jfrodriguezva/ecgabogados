using Dapper;
using ECAbogados.Application.Interfaces;
using ECAbogados.Domain.Entities;

namespace ECAbogados.Infrastructure.Persistence.Repositories;

public class MensajeExpedienteRepository(SqlConnectionFactory connectionFactory) : IMensajeExpedienteRepository
{
    public async Task<IReadOnlyList<MensajeExpediente>> GetByCasoIdAsync(int casoId)
    {
        using var connection = await connectionFactory.CreateOpenConnectionAsync();
        var rows = await connection.QueryAsync<MensajeExpediente>("""
            SELECT Id, CasoId, UsuarioId, AutorNombre, AutorRol, Mensaje, FechaEnvio
            FROM dbo.MensajesExpediente WHERE CasoId = @CasoId ORDER BY FechaEnvio
            """, new { CasoId = casoId });
        return rows.ToList();
    }

    public async Task<int> CreateAsync(MensajeExpediente mensaje)
    {
        using var connection = await connectionFactory.CreateOpenConnectionAsync();
        return await connection.ExecuteScalarAsync<int>("""
            INSERT INTO dbo.MensajesExpediente (CasoId, UsuarioId, AutorNombre, AutorRol, Mensaje, FechaEnvio)
            OUTPUT INSERTED.Id VALUES (@CasoId, @UsuarioId, @AutorNombre, @AutorRol, @Mensaje, @FechaEnvio)
            """, mensaje);
    }
}
