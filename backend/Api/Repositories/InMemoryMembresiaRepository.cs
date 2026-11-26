using Api.Models;

namespace Api.Repositories;

/// <summary>
/// Repositorio en memoria para pruebas locales en Swagger antes de conectar SQL Server.
/// </summary>
public class InMemoryMembresiaRepository : IMembresiaRepository
{
    private readonly List<Membresia> _membresias =
    [
        new Membresia
        {
            Id = 1,
            UsuarioId = 3,
            Tipo = "mensual",
            PromoId = null,
            FechaInicio = DateOnly.FromDateTime(DateTime.Today.AddDays(-8)),
            FechaFin = DateOnly.FromDateTime(DateTime.Today.AddDays(22)),
            Estado = "activa",
            CodigoQr = "LYNX-3-DEMOQR01"
        }
    ];

    private readonly List<Pago> _pagos = [];
    private int _nextMembresiaId = 2;
    private int _nextPagoId = 1;
    private readonly object _lock = new();

    public Task<Membresia?> GetByUsuarioIdAsync(int usuarioId)
    {
        lock (_lock)
        {
            return Task.FromResult(_membresias.FirstOrDefault(m => m.UsuarioId == usuarioId));
        }
    }

    public Task<string?> GetPromoTituloByIdAsync(int promoId)
    {
        string? titulo = promoId switch
        {
            1 => "20% menos este mes",
            2 => "Entrena en pareja",
            _ => null
        };
        return Task.FromResult(titulo);
    }

    public Task<Membresia> CreateOrRenewWithPagoAsync(Membresia membresia, Pago pago)
    {
        lock (_lock)
        {
            var existente = _membresias.FirstOrDefault(m => m.UsuarioId == membresia.UsuarioId);
            if (existente is null)
            {
                membresia.Id = _nextMembresiaId++;
                _membresias.Add(membresia);
            }
            else
            {
                // Se conserva el CodigoQr fijo del socio al renovar.
                existente.Tipo = membresia.Tipo;
                existente.PromoId = membresia.PromoId;
                existente.FechaInicio = membresia.FechaInicio;
                existente.FechaFin = membresia.FechaFin;
                existente.Estado = membresia.Estado;
                membresia = existente;
            }

            pago.Id = _nextPagoId++;
            pago.MembresiaId = membresia.Id;
            _pagos.Add(pago);

            return Task.FromResult(membresia);
        }
    }
}
