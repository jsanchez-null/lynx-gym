using Api.Models;

namespace Api.Repositories;

public interface IPromoRepository
{
    Task<IReadOnlyList<Promo>> GetVigentesAsync(DateOnly hoy);
}
