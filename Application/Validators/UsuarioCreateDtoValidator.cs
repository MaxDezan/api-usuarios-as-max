using FluentValidation;
using APIUsuarios.Application.DTOs;
using APIUsuarios.Application.Interfaces;

namespace APIUsuarios.Application.Validators;

public class UsuarioCreateDtoValidator : AbstractValidator<UsuarioCreateDto>
{
    public UsuarioCreateDtoValidator(IUsuarioRepository repo)
    {
        RuleFor(x => x.Nome)
            .NotEmpty()
            .Length(3, 100);

        RuleFor(x => x.Email)
            .NotEmpty()
            .EmailAddress()
            .MustAsync(async (email, ct) =>
                !await repo.EmailExistsAsync(email.Trim().ToLowerInvariant(), ct))
            .WithMessage("Email já cadastrado");

        RuleFor(x => x.Senha)
            .NotEmpty()
            .MinimumLength(6);

        RuleFor(x => x.DataNascimento)
            .NotEmpty()
            .Must(BeAtLeast18)
            .WithMessage("Usuário deve ter pelo menos 18 anos");

        RuleFor(x => x.Telefone)
            .Matches(@"^\(\d{2}\)\s?\d{4,5}-\d{4}$")
            .When(x => !string.IsNullOrWhiteSpace(x.Telefone));
    }

    private bool BeAtLeast18(DateTime data)
    {
        var hoje = DateTime.Today;
        var idade = hoje.Year - data.Year;
        if (data.Date > hoje.AddYears(-idade))
            idade--;

        return idade >= 18;
    }
}