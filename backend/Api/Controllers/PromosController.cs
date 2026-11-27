using Api.Models.DTOs;
using Api.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers;

[ApiController]
[Route("api/promos")]
public class PromosController : ControllerBase
{
    private readonly PromoService _promoService;

    public PromosController(PromoService promoService)
    {
        _promoService = promoService;
    }

    [HttpGet]
    [Authorize]
    public async Task<ActionResult<ApiResponse<IReadOnlyList<PromoDto>>>> ObtenerVigentes()
    {
        var promos = await _promoService.ObtenerVigentesAsync();
        return Ok(ApiResponse<IReadOnlyList<PromoDto>>.Success(promos));
    }
}
