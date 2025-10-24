using FluentValidation;

namespace ETL.Application.User.Edit;

public class EditUserCommandValidator : AbstractValidator<EditUserCommand>
{
    public EditUserCommandValidator()
    {
        RuleFor(x => x.UserId)
            .NotEmpty()
            .WithMessage("UserId is required.");

        RuleFor(x => x)
            .Must(cmd =>
                !string.IsNullOrEmpty(cmd.Username) ||
                !string.IsNullOrEmpty(cmd.Email) ||
                !string.IsNullOrEmpty(cmd.FirstName) ||
                !string.IsNullOrEmpty(cmd.LastName))
            .WithMessage("At least one updatable field must be provided.");
    }
}
