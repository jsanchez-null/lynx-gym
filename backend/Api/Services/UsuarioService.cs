using Api.Middleware;
using Api.Models;
using Api.Models.DTOs;
using Api.Repositories;

namespace Api.Services;

public class UsuarioService
{
    private readonly IUsuarioRepository _usuarioRepo;

    public UsuarioService(IUsuarioRepository usuarioRepo)
    {
        _usuarioRepo = usuarioRepo;
    }

    public async Task<UsuarioDto> CrearSocioAsync(CrearUsuarioRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.Nombre) ||
            string.IsNullOrWhiteSpace(request.Correo) ||
            string.IsNullOrWhiteSpace(request.Contrasena))
        {
            throw new ApiException(StatusCodes.Status400BadRequest, "Faltan campos obligatorios.");
        }

        var correoNormalizado = request.Correo.Trim().ToLowerInvariant();
        var existente = await _usuarioRepo.GetByCorreoAsync(correoNormalizado);
        if (existente is not null)
        {
            throw new ApiException(StatusCodes.Status409Conflict, "Ya existe un usuario con ese correo.");
        }

        var usuario = new Usuario
        {
            Nombre = request.Nombre.Trim(),
            Correo = correoNormalizado,
            ContrasenaHash = BCrypt.Net.BCrypt.HashPassword(request.Contrasena),
            Rol = "socio",
            Telefono = string.IsNullOrWhiteSpace(request.Telefono) ? null : request.Telefono.Trim()
        };

        var creado = await _usuarioRepo.CreateAsync(usuario);

        return new UsuarioDto
        {
            Id = creado.Id,
            Nombre = creado.Nombre,
            Correo = creado.Correo,
            Rol = creado.Rol,
            Telefono = creado.Telefono
        };
    }
}
