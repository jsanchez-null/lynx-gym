namespace LynxGym.Api.Models;

public class Usuario
{
    public int Id { get; set; }
    public string Nombre { get; set; } = "";
    public string Correo { get; set; } = "";          // único
    public string ContrasenaHash { get; set; } = "";  // BCrypt + salt, nunca en texto plano, nunca se devuelve
    public string Rol { get; set; } = "";             // "socio" | "trabajador" | "administrador"
    public string? Telefono { get; set; }             // opcional
    public DateTime CreadoEn { get; set; }
}

