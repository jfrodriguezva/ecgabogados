namespace ECAbogados.Domain.Entities;

public class MensajeExpediente
{
    public int Id { get; set; }
    public int CasoId { get; set; }
    public int UsuarioId { get; set; }
    public string AutorNombre { get; set; } = string.Empty;
    public string AutorRol { get; set; } = string.Empty;
    public string Mensaje { get; set; } = string.Empty;
    public DateTime FechaEnvio { get; set; }
}
