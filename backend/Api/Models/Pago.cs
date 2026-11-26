namespace Api.Models;

public class Pago
{
    public int Id { get; set; }
    public int MembresiaId { get; set; }
    public decimal Monto { get; set; }
    public DateOnly FechaPago { get; set; }
    public string Metodo { get; set; } = ""; // "efectivo" | "transferencia"
    public int RegistradoPor { get; set; }
}
