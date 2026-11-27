using System.Data;
using Api.Models;
using Microsoft.Data.SqlClient;

namespace Api.Repositories;

public class SqlPromoRepository : IPromoRepository
{
    private readonly string _connectionString;

    public SqlPromoRepository(IConfiguration configuration)
    {
        _connectionString = configuration.GetConnectionString("DefaultConnection")
            ?? throw new InvalidOperationException("Falta ConnectionStrings:DefaultConnection.");
    }

    public async Task<IReadOnlyList<Promo>> GetVigentesAsync(DateOnly hoy)
    {
        const string sql = """
            SELECT id, titulo, descripcion, descuento, fecha_inicio, fecha_fin, creado_por, estado
            FROM Promos
            WHERE estado = 'vigente'
              AND fecha_inicio <= @Hoy
              AND fecha_fin >= @Hoy
            ORDER BY fecha_fin ASC, titulo ASC;
            """;

        await using var conn = new SqlConnection(_connectionString);
        await using var cmd = new SqlCommand(sql, conn);
        cmd.Parameters.Add("@Hoy", SqlDbType.Date).Value = hoy.ToDateTime(TimeOnly.MinValue);

        await conn.OpenAsync();
        await using var reader = await cmd.ExecuteReaderAsync();

        var promos = new List<Promo>();
        while (await reader.ReadAsync())
        {
            promos.Add(new Promo
            {
                Id = reader.GetInt32(0),
                Titulo = reader.GetString(1),
                Descripcion = reader.IsDBNull(2) ? null : reader.GetString(2),
                Descuento = reader.GetDecimal(3),
                FechaInicio = DateOnly.FromDateTime(reader.GetDateTime(4)),
                FechaFin = DateOnly.FromDateTime(reader.GetDateTime(5)),
                CreadoPor = reader.GetInt32(6),
                Estado = reader.GetString(7)
            });
        }

        return promos;
    }
}
