using Api.Models;

namespace Api.Repositories;

public interface IMembresiaRepository
{
    Task<Membresia?> GetByUsuarioIdAsync(int usuarioId);
    Task<string?> GetPromoTituloByIdAsync(int promoId);
    Task<Membresia> CreateOrRenewWithPagoAsync(Membresia membresia, Pago pago);
}
