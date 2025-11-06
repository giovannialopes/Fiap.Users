using FluentValidation;
using Users.Domain.DTO;

namespace Users.Domain.Utils;

public class PerfilRequestValidator : AbstractValidator<PerfilDto.PerfilRequest>
{
    public PerfilRequestValidator() {
        RuleFor(x => x.Nome)
            .NotEmpty().WithMessage("O nome é obrigatório.")
            .MaximumLength(100).WithMessage("O nome não pode ter mais de 100 caracteres.");

        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("O e-mail é obrigatório.")
            .EmailAddress().WithMessage("O e-mail deve ser válido.");

        RuleFor(x => x.SenhaHash)
            .NotEmpty().WithMessage("A senha é obrigatória.");
    }
}
