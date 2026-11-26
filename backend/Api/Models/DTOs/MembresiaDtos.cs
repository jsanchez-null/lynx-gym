namespace Api.Models.DTOs;

public class CrearMembresiaRequest
{
    public int UsuarioId { get; set; }
    public string Tipo { get; set; } = ""; // "mensual" | "anual" | "familiar"
    public int? PromoId { get; set; }      // nulo = "sin promo"
    public decimal Monto { get; set; }
    public string Metodo { get; set; } = ""; // "efectivo" | "transferencia"
}

public class MembresiaDto
{
    public int Id { get; set; }
    public string Tipo { get; set; } = "";
    public string? PromoTitulo { get; set; }
    public DateOnly FechaInicio { get; set; }
    public DateOnly FechaFin { get; set; }
    public string Estado { get; set; } = "";
    public int DiasRestantes { get; set; }
    public string CodigoQr { get; set; } = "";
}
