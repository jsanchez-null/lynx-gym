using Api.Auth;
using Api.Middleware;
using Api.Models.DTOs;
using Api.Repositories;

namespace Api.Services;

public class AuthService
{
    private readonly IUsuarioRepository _usuarioRepo;
    private readonly JwtTokenService _jwtTokenService;

    public AuthService(IUsuarioRepository usuarioRepo, JwtTokenService jwtTokenService)
    {
        _usuarioRepo = usuarioRepo;
        _jwtTokenService = jwtTokenService;
    }

    public async Task<LoginResponse> LoginAsync(LoginRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.Correo) || string.IsNullOrWhiteSpace(request.Contrasena))
        {
            throw new ApiException(StatusCodes.Status400BadRequest, "Faltan campos obligatorios.");
        }

        var correoNormalizado = request.Correo.Trim().ToLowerInvariant();
        var usuario = await _usuarioRepo.GetByCorreoAsync(correoNormalizado);

        if (usuario is null || !BCrypt.Net.BCrypt.Verify(request.Contrasena, usuario.ContrasenaHash))
        {
            throw new ApiException(StatusCodes.Status401Unauthorized, "Correo o contraseña incorrectos.");
        }

        var token = _jwtTokenService.GenerateToken(usuario);

        return new LoginResponse
        {
            Token = token,
            Usuario = new UsuarioDto
            {
                Id = usuario.Id,
                Nombre = usuario.Nombre,
                Correo = usuario.Correo,
                Rol = usuario.Rol,
                Telefono = usuario.Telefono
            }
        };
    }
}
