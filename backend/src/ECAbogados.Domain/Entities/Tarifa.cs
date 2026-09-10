namespace ECAbogados.Domain.Entities;

public class Tarifa
{
    public int Id { get; set; }
    public string Area { get; set; } = string.Empty;
    public string Servicio { get; set; } = string.Empty;
    public string Concepto { get; set; } = string.Empty;
    public decimal MontoBase { get; set; }
    public bool Activa { get; set; } = true;
}
