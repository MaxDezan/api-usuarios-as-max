using APIUsuarios.Application.DTOs;
using APIUsuarios.Application.Interfaces;
using APIUsuarios.Domain.Entities;

namespace APIUsuarios.Application.Services;

public class UsuarioService : IUsuarioService
{
    private readonly IUsuarioRepository _repo;

    public UsuarioService(IUsuarioRepository repo)
    {
        _repo = repo;
    }

    public async Task<UsuarioReadDto> CreateAsync(UsuarioCreateDto dto, CancellationToken ct)
    {
        var usuario = new Usuario
        {
            Nome = dto.Nome,
            Email = dto.Email.Trim().ToLowerInvariant(),
            Senha = dto.Senha,
            DataNascimento = dto.DataNascimento,
            Telefone = dto.Telefone
        };

        await _repo.AddAsync(usuario, ct);
        await _repo.SaveChangesAsync(ct);

        return new UsuarioReadDto(
            usuario.Id, usuario.Nome, usuario.Email,
            usuario.DataNascimento, usuario.Telefone,
            usuario.Ativo, usuario.DataCriacao
        );
    }

    public async Task<IEnumerable<UsuarioReadDto>> ListAsync(CancellationToken ct)
    {
        var lista = await _repo.GetAllAsync(ct);

        return lista.Select(u => new UsuarioReadDto(
            u.Id, u.Nome, u.Email, u.DataNascimento,
            u.Telefone, u.Ativo, u.DataCriacao
        ));
    }

    public async Task<UsuarioReadDto?> GetAsync(int id, CancellationToken ct)
    {
        var usuario = await _repo.GetByIdAsync(id, ct);
        if (usuario is null) return null;

        return new UsuarioReadDto(
            usuario.Id, usuario.Nome, usuario.Email,
            usuario.DataNascimento, usuario.Telefone,
            usuario.Ativo, usuario.DataCriacao
        );
    }

    public async Task<UsuarioReadDto?> UpdateAsync(int id, UsuarioUpdateDto dto, CancellationToken ct)
    {
        var usuario = await _repo.GetByIdAsync(id, ct);
        if (usuario is null) return null;

        usuario.Nome = dto.Nome;
        usuario.Email = dto.Email.Trim().ToLowerInvariant();
        usuario.DataNascimento = dto.DataNascimento;
        usuario.Telefone = dto.Telefone;
        usuario.Ativo = dto.Ativo;
        usuario.DataAtualizacao = DateTime.UtcNow;

        await _repo.UpdateAsync(usuario, ct);
        await _repo.SaveChangesAsync(ct);

        return new UsuarioReadDto(
            usuario.Id, usuario.Nome, usuario.Email,
            usuario.DataNascimento, usuario.Telefone,
            usuario.Ativo, usuario.DataCriacao
        );
    }

    public async Task<bool> RemoveAsync(int id, CancellationToken ct)
    {
        var usuario = await _repo.GetByIdAsync(id, ct);
        if (usuario is null) return false;

        usuario.Ativo = false;
        usuario.DataAtualizacao = DateTime.UtcNow;

        await _repo.UpdateAsync(usuario, ct);
        await _repo.SaveChangesAsync(ct);

        return true;
    }
}
