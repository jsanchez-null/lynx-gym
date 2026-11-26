namespace Api.Models.DTOs;

public class LoginRequest
{
    public string Correo { get; set; } = "";
    public string Contrasena { get; set; } = "";
}

public class LoginResponse
{
    public string Token { get; set; } = "";
    public UsuarioDto Usuario { get; set; } = null!;
}

public class UsuarioDto
{
    public int Id { get; set; }
    public string Nombre { get; set; } = "";
    public string Correo { get; set; } = "";
    public string Rol { get; set; } = "";
    public string? Telefono { get; set; }
}
