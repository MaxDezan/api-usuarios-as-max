using APIUsuarios.Application.DTOs;

namespace APIUsuarios.Application.Interfaces;

public interface IUsuarioService
{
    Task<IEnumerable<UsuarioReadDto>> ListAsync(CancellationToken ct);
    Task<UsuarioReadDto?> GetAsync(int id, CancellationToken ct);
    Task<UsuarioReadDto> CreateAsync(UsuarioCreateDto dto, CancellationToken ct);
    Task<UsuarioReadDto?> UpdateAsync(int id, UsuarioUpdateDto dto, CancellationToken ct);
    Task<bool> RemoveAsync(int id, CancellationToken ct);
}