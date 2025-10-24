using ETL.Application.Common.Constants;
using FluentValidation;

namespace ETL.Application.User.Create;

public class CreateUserCommandValidator : AbstractValidator<CreateUserCommand>
{
    public CreateUserCommandValidator()
    {
        RuleFor(x => x.Username)
            .NotEmpty()
            .WithMessage("Username is required.");

        RuleFor(x => x.Password)
            .NotEmpty()
            .WithMessage("Password is required.");

        RuleFor(x => x.Role)
            .Must(role => Role.GetAllRoles().Contains(role))
            .WithMessage("Role must be one of the known roles.");
    }
}
