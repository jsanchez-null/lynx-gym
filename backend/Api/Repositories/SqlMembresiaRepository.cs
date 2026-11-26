using System.Data;
using Api.Models;
using Microsoft.Data.SqlClient;

namespace Api.Repositories;

/// <summary>
/// Repositorio SQL Server para las tablas Membresias, Promos (lectura de título) y Pagos.
/// </summary>
public class SqlMembresiaRepository : IMembresiaRepository
{
    private readonly string _connectionString;

    public SqlMembresiaRepository(IConfiguration configuration)
    {
        _connectionString = configuration.GetConnectionString("DefaultConnection")
            ?? throw new InvalidOperationException("Falta ConnectionStrings:DefaultConnection.");
    }

    public async Task<Membresia?> GetByUsuarioIdAsync(int usuarioId)
    {
        const string sql = """
            SELECT id, usuario_id, tipo, promo_id, fecha_inicio, fecha_fin, estado, codigo_qr
            FROM Membresias
            WHERE usuario_id = @UsuarioId;
            """;

        await using var conn = new SqlConnection(_connectionString);
        await using var cmd = new SqlCommand(sql, conn);
        cmd.Parameters.Add("@UsuarioId", SqlDbType.Int).Value = usuarioId;

        await conn.OpenAsync();
        await using var reader = await cmd.ExecuteReaderAsync();
        if (!await reader.ReadAsync())
        {
            return null;
        }

        return new Membresia
        {
            Id = reader.GetInt32(0),
            UsuarioId = reader.GetInt32(1),
            Tipo = reader.GetString(2),
            PromoId = reader.IsDBNull(3) ? null : reader.GetInt32(3),
            FechaInicio = DateOnly.FromDateTime(reader.GetDateTime(4)),
            FechaFin = DateOnly.FromDateTime(reader.GetDateTime(5)),
            Estado = reader.GetString(6),
            CodigoQr = reader.GetString(7)
        };
    }

    public async Task<string?> GetPromoTituloByIdAsync(int promoId)
    {
        const string sql = "SELECT titulo FROM Promos WHERE id = @PromoId;";

        await using var conn = new SqlConnection(_connectionString);
        await using var cmd = new SqlCommand(sql, conn);
        cmd.Parameters.Add("@PromoId", SqlDbType.Int).Value = promoId;

        await conn.OpenAsync();
        var result = await cmd.ExecuteScalarAsync();
        return result is DBNull or null ? null : Convert.ToString(result);
    }

    public async Task<Membresia> CreateOrRenewWithPagoAsync(Membresia membresia, Pago pago)
    {
        await using var conn = new SqlConnection(_connectionString);
        await conn.OpenAsync();
        await using var tx = (SqlTransaction)await conn.BeginTransactionAsync();

        try
        {
            const string findSql = "SELECT id, codigo_qr FROM Membresias WHERE usuario_id = @UsuarioId;";
            int? existingId = null;
            string? existingQr = null;

            await using (var findCmd = new SqlCommand(findSql, conn, tx))
            {
                findCmd.Parameters.Add("@UsuarioId", SqlDbType.Int).Value = membresia.UsuarioId;
                await using var reader = await findCmd.ExecuteReaderAsync();
                if (await reader.ReadAsync())
                {
                    existingId = reader.GetInt32(0);
                    existingQr = reader.GetString(1);
                }
            }

            if (existingId is null)
            {
                const string insertSql = """
                    INSERT INTO Membresias (usuario_id, tipo, promo_id, fecha_inicio, fecha_fin, estado, codigo_qr)
                    OUTPUT INSERTED.id
                    VALUES (@UsuarioId, @Tipo, @PromoId, @FechaInicio, @FechaFin, @Estado, @CodigoQr);
                    """;

                await using var insertCmd = new SqlCommand(insertSql, conn, tx);
                insertCmd.Parameters.Add("@UsuarioId", SqlDbType.Int).Value = membresia.UsuarioId;
                insertCmd.Parameters.Add("@Tipo", SqlDbType.NVarChar, 30).Value = membresia.Tipo;
                insertCmd.Parameters.Add("@PromoId", SqlDbType.Int).Value = (object?)membresia.PromoId ?? DBNull.Value;
                insertCmd.Parameters.Add("@FechaInicio", SqlDbType.Date).Value = membresia.FechaInicio.ToDateTime(TimeOnly.MinValue);
                insertCmd.Parameters.Add("@FechaFin", SqlDbType.Date).Value = membresia.FechaFin.ToDateTime(TimeOnly.MinValue);
                insertCmd.Parameters.Add("@Estado", SqlDbType.NVarChar, 30).Value = membresia.Estado;
                insertCmd.Parameters.Add("@CodigoQr", SqlDbType.NVarChar, 120).Value = membresia.CodigoQr;

                membresia.Id = Convert.ToInt32(await insertCmd.ExecuteScalarAsync());
            }
            else
            {
                const string updateSql = """
                    UPDATE Membresias
                    SET tipo = @Tipo,
                        promo_id = @PromoId,
                        fecha_inicio = @FechaInicio,
                        fecha_fin = @FechaFin,
                        estado = @Estado
                    WHERE id = @Id;
                    """;

                await using var updateCmd = new SqlCommand(updateSql, conn, tx);
                updateCmd.Parameters.Add("@Id", SqlDbType.Int).Value = existingId.Value;
                updateCmd.Parameters.Add("@Tipo", SqlDbType.NVarChar, 30).Value = membresia.Tipo;
                updateCmd.Parameters.Add("@PromoId", SqlDbType.Int).Value = (object?)membresia.PromoId ?? DBNull.Value;
                updateCmd.Parameters.Add("@FechaInicio", SqlDbType.Date).Value = membresia.FechaInicio.ToDateTime(TimeOnly.MinValue);
                updateCmd.Parameters.Add("@FechaFin", SqlDbType.Date).Value = membresia.FechaFin.ToDateTime(TimeOnly.MinValue);
                updateCmd.Parameters.Add("@Estado", SqlDbType.NVarChar, 30).Value = membresia.Estado;
                await updateCmd.ExecuteNonQueryAsync();

                membresia.Id = existingId.Value;
                membresia.CodigoQr = existingQr!;
            }

            const string pagoSql = """
                INSERT INTO Pagos (membresia_id, monto, fecha_pago, metodo, registrado_por)
                OUTPUT INSERTED.id
                VALUES (@MembresiaId, @Monto, @FechaPago, @Metodo, @RegistradoPor);
                """;

            await using (var pagoCmd = new SqlCommand(pagoSql, conn, tx))
            {
                pagoCmd.Parameters.Add("@MembresiaId", SqlDbType.Int).Value = membresia.Id;
                pagoCmd.Parameters.Add("@Monto", SqlDbType.Decimal).Value = pago.Monto;
                pagoCmd.Parameters.Add("@FechaPago", SqlDbType.Date).Value = pago.FechaPago.ToDateTime(TimeOnly.MinValue);
                pagoCmd.Parameters.Add("@Metodo", SqlDbType.NVarChar, 30).Value = pago.Metodo;
                pagoCmd.Parameters.Add("@RegistradoPor", SqlDbType.Int).Value = pago.RegistradoPor;
                pago.Id = Convert.ToInt32(await pagoCmd.ExecuteScalarAsync());
                pago.MembresiaId = membresia.Id;
            }

            await tx.CommitAsync();
            return membresia;
        }
        catch
        {
            await tx.RollbackAsync();
            throw;
        }
    }
}
