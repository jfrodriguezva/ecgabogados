using ECAbogados.Application.Mediation;

namespace ECAbogados.Application.Citas.Commands.CrearCita;

public record CrearCitaCommand(
    int? CasoId,
    string NombreCliente,
    string Telefono,
    string? Email,
    string Servicio,
    string Modalidad,
    string? Comentario,
    DateTime FechaHora) : IRequest<int>;
