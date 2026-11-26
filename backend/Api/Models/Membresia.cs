namespace LynxGym.Api.Models;

public class Membresia
{
    public int Id { get; set; }
    public int UsuarioId { get; set; }
    public string Tipo { get; set; } = "";            // "mensual" | "anual" | "familiar"
    public int? PromoId { get; set; }                 // nulo = "sin promo"
    public DateOnly FechaInicio { get; set; }
    public DateOnly FechaFin { get; set; }             // vigencia
    public string Estado { get; set; } = "";          // "activa" | "vencida" | "suspendida"
    public string CodigoQr { get; set; } = "";        // único, fijo por membresía
}

