using FluentValidation;

namespace ETL.Application.DataSet.RenameTable;

public sealed class RenameTableCommandValidator : AbstractValidator<RenameTableCommand>
{
    private const int MaxIdentifierLength = 63;
    private const string AllowedPattern = @"^\w+$";

    public RenameTableCommandValidator()
    {
        RuleFor(x => x.OldTableName)
            .Cascade(CascadeMode.Stop)
            .NotEmpty().WithMessage("Old table name must be provided.")
            .MaximumLength(MaxIdentifierLength).WithMessage($"Old table name must be at most {MaxIdentifierLength} characters.")
            .Matches(AllowedPattern).WithMessage("Old table name may contain only letters, digits and underscore.");

        RuleFor(x => x.NewTableName)
            .Cascade(CascadeMode.Stop)
            .NotEmpty().WithMessage("New table name must be provided.")
            .MaximumLength(MaxIdentifierLength).WithMessage($"New table name must be at most {MaxIdentifierLength} characters.")
            .Matches(AllowedPattern).WithMessage("New table name may contain only letters, digits and underscore.");

        RuleFor(x => x)
            .Must(cmd => !string.Equals(cmd.OldTableName?.Trim(), cmd.NewTableName?.Trim(), StringComparison.OrdinalIgnoreCase))
            .WithMessage("New table name must be different from the old table name.");
    }
}
