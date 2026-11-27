using Api.Models.DTOs;
using Api.Repositories;

namespace Api.Services;

public class PromoService
{
    private readonly IPromoRepository _promoRepository;

    public PromoService(IPromoRepository promoRepository)
    {
        _promoRepository = promoRepository;
    }

    public async Task<IReadOnlyList<PromoDto>> ObtenerVigentesAsync()
    {
        var hoy = DateOnly.FromDateTime(DateTime.Today);
        var promos = await _promoRepository.GetVigentesAsync(hoy);

        return promos.Select(promo => new PromoDto
        {
            Id = promo.Id,
            Titulo = promo.Titulo,
            Descripcion = promo.Descripcion,
            Descuento = promo.Descuento,
            FechaInicio = promo.FechaInicio,
            FechaFin = promo.FechaFin
        }).ToList();
    }
}
