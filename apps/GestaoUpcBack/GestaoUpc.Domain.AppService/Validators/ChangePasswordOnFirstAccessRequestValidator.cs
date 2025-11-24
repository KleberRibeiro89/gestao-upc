using FluentValidation;
using GestaoUpc.Domain.DTOs.Requests.UserRequestSource;

namespace GestaoUpc.Domain.AppService.Validators;

public class ChangePasswordOnFirstAccessRequestValidator : AbstractValidator<ChangePasswordOnFirstAccessRequest>
{
    public ChangePasswordOnFirstAccessRequestValidator()
    {
        RuleFor(x => x.UserId)
            .NotEmpty()
            .WithMessage("ID do usuário é obrigatório");

        RuleFor(x => x.NewPassword)
            .NotEmpty()
            .WithMessage("Nova senha é obrigatória")
            .MinimumLength(6)
            .WithMessage("A senha deve ter no mínimo 6 caracteres")
            .MaximumLength(500)
            .WithMessage("Senha deve ter no máximo 500 caracteres");
    }
}

