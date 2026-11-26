namespace Api.Models.DTOs;

public class CrearUsuarioRequest
{
    public string Nombre { get; set; } = "";
    public string Correo { get; set; } = "";
    public string Contrasena { get; set; } = "";
    public string? Telefono { get; set; }
}
