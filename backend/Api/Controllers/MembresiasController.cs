using System.Security.Claims;
using Api.Middleware;
using Api.Models.DTOs;
using Api.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers;

[ApiController]
public class MembresiasController : ControllerBase
{
    private readonly MembresiaService _membresiaService;

    public MembresiasController(MembresiaService membresiaService)
    {
        _membresiaService = membresiaService;
    }

    [HttpPost("api/admin/membresias")]
    [Authorize(Roles = "trabajador,administrador")]
    public async Task<ActionResult<ApiResponse<MembresiaDto>>> AsignarORenovar(
        [FromBody] CrearMembresiaRequest request)
    {
        var userId = GetCurrentUserId();
        var membresia = await _membresiaService.AsignarORenovarAsync(request, userId);
        return StatusCode(StatusCodes.Status201Created, ApiResponse<MembresiaDto>.Success(membresia));
    }

    [HttpGet("api/membresias/mia")]
    [Authorize]
    public async Task<ActionResult<ApiResponse<MembresiaDto>>> ObtenerMia()
    {
        var userId = GetCurrentUserId();
        var membresia = await _membresiaService.ObtenerMiaAsync(userId);
        return Ok(ApiResponse<MembresiaDto>.Success(membresia));
    }

    private int GetCurrentUserId()
    {
        var rawId = User.FindFirstValue("id") ?? User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (!int.TryParse(rawId, out var userId))
        {
            throw new ApiException(StatusCodes.Status401Unauthorized, "Sesión no válida. Vuelve a iniciar sesión.");
        }

        return userId;
    }
}
