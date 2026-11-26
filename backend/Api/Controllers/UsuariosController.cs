using Api.Models.DTOs;
using Api.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers;

[ApiController]
[Route("api/usuarios")]
public class UsuariosController : ControllerBase
{
    private readonly UsuarioService _usuarioService;

    public UsuariosController(UsuarioService usuarioService)
    {
        _usuarioService = usuarioService;
    }

    [HttpPost]
    [Authorize(Roles = "trabajador,administrador")]
    public async Task<ActionResult<ApiResponse<UsuarioDto>>> CrearSocio([FromBody] CrearUsuarioRequest request)
    {
        var creado = await _usuarioService.CrearSocioAsync(request);
        return StatusCode(StatusCodes.Status201Created, ApiResponse<UsuarioDto>.Success(creado));
    }
}
