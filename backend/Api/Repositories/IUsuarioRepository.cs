using Api.Models;

namespace Api.Repositories;

public interface IUsuarioRepository
{
    Task<Usuario?> GetByCorreoAsync(string correo);
    Task<Usuario?> GetByIdAsync(int id);
    Task<Usuario> CreateAsync(Usuario usuario);
}
