namespace Api.Models;

public class Promo
{
    public int Id { get; set; }
    public string Titulo { get; set; } = "";
    public string? Descripcion { get; set; }
    public decimal Descuento { get; set; }
    public DateOnly FechaInicio { get; set; }
    public DateOnly FechaFin { get; set; }
    public int CreadoPor { get; set; }
    public string Estado { get; set; } = ""; // "vigente" | "vencida"
}
