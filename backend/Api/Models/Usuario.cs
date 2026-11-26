namespace Api.Models;

public class Usuario
{
    public int Id { get; set; }
    public string Nombre { get; set; } = "";
    public string Correo { get; set; } = "";
    public string ContrasenaHash { get; set; } = "";
    public string Rol { get; set; } = ""; // "socio" | "trabajador" | "administrador"
    public string? Telefono { get; set; }
    public DateTime CreadoEn { get; set; }
}
