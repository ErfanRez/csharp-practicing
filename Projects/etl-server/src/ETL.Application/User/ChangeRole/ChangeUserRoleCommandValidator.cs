using ETL.Application.Common.Constants;
using FluentValidation;

namespace ETL.Application.User.ChangeRole;

public class ChangeUserRoleCommandValidator : AbstractValidator<ChangeUserRoleCommand>
{
    public ChangeUserRoleCommandValidator()
    {
        RuleFor(x => x.UserId)
            .NotEmpty()
            .WithMessage("UserId is required.");

        RuleFor(x => x.Role)
            .Must(role => Role.GetAllRoles().Contains(role))
            .WithMessage("Role must be one of the known roles.");
    }
}
