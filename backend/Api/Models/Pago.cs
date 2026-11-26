namespace LynxGym.Api.Models;

public class Pago
{
    public int Id { get; set; }
    public int MembresiaId { get; set; }
    public decimal Monto { get; set; }                // monto pagado en el gym (con descuento ya aplicado)
    public DateOnly FechaPago { get; set; }
    public string Metodo { get; set; } = "";          // "efectivo" | "transferencia"
    public int RegistradoPor { get; set; }            // FK -> Usuarios
}

