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
        // Normalização e regras de negócio mínimas
        var normalizedEmail = dto.Email.Trim().ToLowerInvariant();

        // Checagem de unicidade de email (negócio)
        if (await _repo.EmailExistsAsync(normalizedEmail, ct))
        {
            throw new InvalidOperationException("EMAIL_DUPLICADO");
        }

        var usuario = new Usuario
        {
            Nome = dto.Nome,
            Email = normalizedEmail,
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

        var normalizedEmail = dto.Email.Trim().ToLowerInvariant();

        // Se o email mudou, verificar duplicidade
        if (!string.Equals(usuario.Email, normalizedEmail, StringComparison.OrdinalIgnoreCase))
        {
            var jaExiste = await _repo.GetByEmailAsync(normalizedEmail, ct);
            if (jaExiste is not null && jaExiste.Id != id)
            {
                throw new InvalidOperationException("EMAIL_DUPLICADO");
            }
        }

        usuario.Nome = dto.Nome;
        usuario.Email = normalizedEmail;
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

    public Task<bool> EmailJaCadastradoAsync(string email, CancellationToken ct)
    {
        var normalized = email.Trim().ToLowerInvariant();
        return _repo.EmailExistsAsync(normalized, ct);
    }
}
