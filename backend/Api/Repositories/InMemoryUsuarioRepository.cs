using Api.Models;

namespace Api.Repositories;

/// <summary>
/// Repositorio en memoria para pruebas locales en Swagger antes de conectar SQL Server.
/// </summary>
public class InMemoryUsuarioRepository : IUsuarioRepository
{
    private readonly List<Usuario> _usuarios =
    [
        new Usuario
        {
            Id = 1,
            Nombre = "Admin Lynx",
            Correo = "admin@lynxgym.com",
            ContrasenaHash = BCrypt.Net.BCrypt.HashPassword("Admin123!"),
            Rol = "administrador",
            Telefono = "6620000001",
            CreadoEn = DateTime.UtcNow
        },
        new Usuario
        {
            Id = 2,
            Nombre = "Recepción Lynx",
            Correo = "trabajador@lynxgym.com",
            ContrasenaHash = BCrypt.Net.BCrypt.HashPassword("Trabajador123!"),
            Rol = "trabajador",
            Telefono = "6620000002",
            CreadoEn = DateTime.UtcNow
        },
        new Usuario
        {
            Id = 3,
            Nombre = "Alex Socio",
            Correo = "socio@lynxgym.com",
            ContrasenaHash = BCrypt.Net.BCrypt.HashPassword("Socio123!"),
            Rol = "socio",
            Telefono = "6620000003",
            CreadoEn = DateTime.UtcNow
        }
    ];

    private int _nextId = 4;
    private readonly object _lock = new();

    public Task<Usuario?> GetByCorreoAsync(string correo)
    {
        lock (_lock)
        {
            var user = _usuarios.FirstOrDefault(u =>
                u.Correo.Equals(correo, StringComparison.OrdinalIgnoreCase));
            return Task.FromResult(user);
        }
    }

    public Task<Usuario?> GetByIdAsync(int id)
    {
        lock (_lock)
        {
            return Task.FromResult(_usuarios.FirstOrDefault(u => u.Id == id));
        }
    }

    public Task<Usuario> CreateAsync(Usuario usuario)
    {
        lock (_lock)
        {
            usuario.Id = _nextId++;
            usuario.CreadoEn = DateTime.UtcNow;
            _usuarios.Add(usuario);
            return Task.FromResult(usuario);
        }
    }
}
