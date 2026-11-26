using Api.Middleware;
using Api.Models;
using Api.Models.DTOs;
using Api.Repositories;

namespace Api.Services;

public class MembresiaService
{
    private static readonly HashSet<string> TiposValidos = ["mensual", "anual", "familiar"];
    private static readonly HashSet<string> MetodosValidos = ["efectivo", "transferencia"];

    private readonly IMembresiaRepository _membresiaRepo;
    private readonly IUsuarioRepository _usuarioRepo;

    public MembresiaService(IMembresiaRepository membresiaRepo, IUsuarioRepository usuarioRepo)
    {
        _membresiaRepo = membresiaRepo;
        _usuarioRepo = usuarioRepo;
    }

    public async Task<MembresiaDto> AsignarORenovarAsync(CrearMembresiaRequest request, int registradoPor)
    {
        var tipo = request.Tipo.Trim().ToLowerInvariant();
        var metodo = request.Metodo.Trim().ToLowerInvariant();

        if (request.UsuarioId <= 0 || !TiposValidos.Contains(tipo) || !MetodosValidos.Contains(metodo) || request.Monto <= 0)
        {
            throw new ApiException(StatusCodes.Status400BadRequest, "Datos de membresía o pago inválidos.");
        }

        var socio = await _usuarioRepo.GetByIdAsync(request.UsuarioId);
        if (socio is null)
        {
            throw new ApiException(StatusCodes.Status404NotFound, "No se encontró el usuario.");
        }

        var hoy = DateOnly.FromDateTime(DateTime.Today);
        var fechaFin = tipo == "anual" ? hoy.AddYears(1) : hoy.AddMonths(1);

        var membresia = new Membresia
        {
            UsuarioId = request.UsuarioId,
            Tipo = tipo,
            PromoId = request.PromoId,
            FechaInicio = hoy,
            FechaFin = fechaFin,
            Estado = "activa",
            CodigoQr = $"LYNX-{request.UsuarioId}-{Guid.NewGuid().ToString("N")[..8].ToUpperInvariant()}"
        };

        var pago = new Pago
        {
            Monto = request.Monto,
            FechaPago = hoy,
            Metodo = metodo,
            RegistradoPor = registradoPor
        };

        var guardada = await _membresiaRepo.CreateOrRenewWithPagoAsync(membresia, pago);
        return await ToDtoAsync(guardada);
    }

    public async Task<MembresiaDto> ObtenerMiaAsync(int usuarioId)
    {
        var membresia = await _membresiaRepo.GetByUsuarioIdAsync(usuarioId);
        if (membresia is null)
        {
            throw new ApiException(StatusCodes.Status404NotFound, "No se encontró una membresía para tu cuenta.");
        }

        return await ToDtoAsync(membresia);
    }

    private async Task<MembresiaDto> ToDtoAsync(Membresia m)
    {
        var hoy = DateOnly.FromDateTime(DateTime.Today);
        var diasRestantes = Math.Max(0, m.FechaFin.DayNumber - hoy.DayNumber);
        string? promoTitulo = m.PromoId.HasValue
            ? await _membresiaRepo.GetPromoTituloByIdAsync(m.PromoId.Value)
            : null;

        return new MembresiaDto
        {
            Id = m.Id,
            Tipo = m.Tipo,
            PromoTitulo = promoTitulo,
            FechaInicio = m.FechaInicio,
            FechaFin = m.FechaFin,
            Estado = diasRestantes == 0 && m.Estado == "activa" ? "vencida" : m.Estado,
            DiasRestantes = diasRestantes,
            CodigoQr = m.CodigoQr
        };
    }
}
