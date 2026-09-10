using ECAbogados.Domain.Entities;

namespace ECAbogados.Application.Dtos;

public record CitaDto(
    int Id,
    int? CasoId,
    string NombreCliente,
    string Telefono,
    string? Email,
    string Servicio,
    string Modalidad,
    string? Comentario,
    DateTime FechaHora,
    EstatusCita Estatus);
