using System.Data;
using Api.Models;
using Microsoft.Data.SqlClient;

namespace Api.Repositories;

/// <summary>
/// Repositorio SQL Server para la tabla Usuarios.
/// </summary>
public class SqlUsuarioRepository : IUsuarioRepository
{
    private readonly string _connectionString;

    public SqlUsuarioRepository(IConfiguration configuration)
    {
        _connectionString = configuration.GetConnectionString("DefaultConnection")
            ?? throw new InvalidOperationException("Falta ConnectionStrings:DefaultConnection.");
    }

    public async Task<Usuario?> GetByCorreoAsync(string correo)
    {
        const string sql = """
            SELECT id, nombre, correo, contrasena_hash, rol, telefono, creado_en
            FROM Usuarios
            WHERE correo = @Correo;
            """;

        await using var conn = new SqlConnection(_connectionString);
        await using var cmd = new SqlCommand(sql, conn);
        cmd.Parameters.Add("@Correo", SqlDbType.NVarChar, 150).Value = correo;

        await conn.OpenAsync();
        await using var reader = await cmd.ExecuteReaderAsync();
        return await reader.ReadAsync() ? MapUsuario(reader) : null;
    }

    public async Task<Usuario?> GetByIdAsync(int id)
    {
        const string sql = """
            SELECT id, nombre, correo, contrasena_hash, rol, telefono, creado_en
            FROM Usuarios
            WHERE id = @Id;
            """;

        await using var conn = new SqlConnection(_connectionString);
        await using var cmd = new SqlCommand(sql, conn);
        cmd.Parameters.Add("@Id", SqlDbType.Int).Value = id;

        await conn.OpenAsync();
        await using var reader = await cmd.ExecuteReaderAsync();
        return await reader.ReadAsync() ? MapUsuario(reader) : null;
    }

    public async Task<Usuario> CreateAsync(Usuario usuario)
    {
        const string sql = """
            INSERT INTO Usuarios (nombre, correo, contrasena_hash, rol, telefono, creado_en)
            OUTPUT INSERTED.id, INSERTED.creado_en
            VALUES (@Nombre, @Correo, @ContrasenaHash, @Rol, @Telefono, SYSUTCDATETIME());
            """;

        await using var conn = new SqlConnection(_connectionString);
        await using var cmd = new SqlCommand(sql, conn);
        cmd.Parameters.Add("@Nombre", SqlDbType.NVarChar, 150).Value = usuario.Nombre;
        cmd.Parameters.Add("@Correo", SqlDbType.NVarChar, 150).Value = usuario.Correo;
        cmd.Parameters.Add("@ContrasenaHash", SqlDbType.NVarChar, 255).Value = usuario.ContrasenaHash;
        cmd.Parameters.Add("@Rol", SqlDbType.NVarChar, 30).Value = usuario.Rol;
        cmd.Parameters.Add("@Telefono", SqlDbType.NVarChar, 30).Value = (object?)usuario.Telefono ?? DBNull.Value;

        await conn.OpenAsync();
        await using var reader = await cmd.ExecuteReaderAsync();
        if (await reader.ReadAsync())
        {
            usuario.Id = reader.GetInt32(0);
            usuario.CreadoEn = reader.GetDateTime(1);
        }

        return usuario;
    }

    private static Usuario MapUsuario(SqlDataReader reader) => new()
    {
        Id = reader.GetInt32(0),
        Nombre = reader.GetString(1),
        Correo = reader.GetString(2),
        ContrasenaHash = reader.GetString(3),
        Rol = reader.GetString(4),
        Telefono = reader.IsDBNull(5) ? null : reader.GetString(5),
        CreadoEn = reader.GetDateTime(6)
    };
}
