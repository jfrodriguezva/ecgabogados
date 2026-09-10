using Dapper;
using ECAbogados.Application.Interfaces;
using ECAbogados.Domain.Entities;
namespace ECAbogados.Infrastructure.Persistence.Repositories;
public class TarifaRepository(SqlConnectionFactory factory) : ITarifaRepository
{
    public async Task<IReadOnlyList<Tarifa>> GetAllAsync()
    {
        using var db = await factory.CreateOpenConnectionAsync();
        var items = await db.QueryAsync<Tarifa>("SELECT Id, Area, Servicio, Concepto, MontoBase, Activa FROM dbo.Tarifas ORDER BY Area, Servicio, Concepto");
        return items.ToList();
    }
    public async Task<int> CreateAsync(Tarifa x)
    {
        using var db = await factory.CreateOpenConnectionAsync();
        return await db.ExecuteScalarAsync<int>("INSERT dbo.Tarifas (Area,Servicio,Concepto,MontoBase,Activa) OUTPUT INSERTED.Id VALUES (@Area,@Servicio,@Concepto,@MontoBase,@Activa)", x);
    }
    public async Task UpdateAsync(Tarifa x)
    {
        using var db = await factory.CreateOpenConnectionAsync();
        await db.ExecuteAsync("UPDATE dbo.Tarifas SET Area=@Area,Servicio=@Servicio,Concepto=@Concepto,MontoBase=@MontoBase,Activa=@Activa WHERE Id=@Id", x);
    }
}
