using FluentValidation;
using GestaoUpc.Domain.DTOs.Requests.UserRequestSource;

namespace GestaoUpc.Domain.AppService.Validators;

public class CreateUserRequestValidator : AbstractValidator<CreateUserRequest>
{
    public CreateUserRequestValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty()
            .WithMessage("Nome é obrigatório")
            .MaximumLength(200)
            .WithMessage("Nome deve ter no máximo 200 caracteres");

        RuleFor(x => x.Email)
            .NotEmpty()
            .WithMessage("Email é obrigatório")
            .EmailAddress()
            .WithMessage("Email inválido")
            .MaximumLength(200)
            .WithMessage("Email deve ter no máximo 200 caracteres");

    }
}

