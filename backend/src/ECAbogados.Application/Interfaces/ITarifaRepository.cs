using ECAbogados.Domain.Entities;
namespace ECAbogados.Application.Interfaces;
public interface ITarifaRepository
{
    Task<IReadOnlyList<Tarifa>> GetAllAsync();
    Task<int> CreateAsync(Tarifa tarifa);
    Task UpdateAsync(Tarifa tarifa);
}
