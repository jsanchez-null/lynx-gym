using Api.Models;

namespace Api.Repositories;

public class InMemoryPromoRepository : IPromoRepository
{
    private readonly List<Promo> _promos =
    [
        new Promo
        {
            Id = 1,
            Titulo = "Promo de bienvenida",
            Descripcion = "Descuento para nuevos socios.",
            Descuento = 10m,
            FechaInicio = DateOnly.FromDateTime(DateTime.Today),
            FechaFin = DateOnly.FromDateTime(DateTime.Today.AddMonths(1)),
            CreadoPor = 1,
            Estado = "vigente"
        }
    ];

    public Task<IReadOnlyList<Promo>> GetVigentesAsync(DateOnly hoy)
    {
        var vigentes = _promos
            .Where(promo => promo.Estado == "vigente"
                            && promo.FechaInicio <= hoy
                            && promo.FechaFin >= hoy)
            .OrderBy(promo => promo.FechaFin)
            .ThenBy(promo => promo.Titulo)
            .ToList();

        return Task.FromResult<IReadOnlyList<Promo>>(vigentes);
    }
}
