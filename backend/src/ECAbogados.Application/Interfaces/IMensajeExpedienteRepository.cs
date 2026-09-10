using ECAbogados.Domain.Entities;

namespace ECAbogados.Application.Interfaces;

public interface IMensajeExpedienteRepository
{
    Task<IReadOnlyList<MensajeExpediente>> GetByCasoIdAsync(int casoId);
    Task<int> CreateAsync(MensajeExpediente mensaje);
}
