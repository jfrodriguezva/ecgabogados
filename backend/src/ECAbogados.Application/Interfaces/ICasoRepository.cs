using ECAbogados.Domain.Entities;

namespace ECAbogados.Application.Interfaces;

public interface ICasoRepository
{
    Task<IReadOnlyList<Caso>> GetAllAsync();
    Task<(IReadOnlyList<Caso> Items, int TotalCount)> GetPagedAsync(int page, int pageSize, string? search);
    Task<Caso?> GetByIdAsync(int id);
    Task<Caso?> GetByTokenAsync(string token);
    Task<IReadOnlyList<Caso>> GetByClienteUsuarioIdAsync(int usuarioId);
    Task<int> CreateAsync(Caso caso);
    Task UpdateAsync(Caso caso);
    Task AsignarClienteYEtapaAsync(int id, int? clienteUsuarioId, string etapa);
    Task<string> RegenerarTokenAsync(int id);
}
