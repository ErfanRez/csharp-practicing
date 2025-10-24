using FluentValidation;

namespace ETL.Application.DataSet.RenameColumn;

public sealed class RenameColumnCommandValidator : AbstractValidator<RenameColumnCommand>
{
    private const int MaxIdentifierLength = 63;
    private const string AllowedPattern = @"^\w+$";

    public RenameColumnCommandValidator()
    {
        RuleFor(x => x.TableName)
            .Cascade(CascadeMode.Stop)
            .NotEmpty().WithMessage("Table name must be provided.")
            .MaximumLength(MaxIdentifierLength).WithMessage($"Table name must be at most {MaxIdentifierLength} characters.")
            .Matches(AllowedPattern).WithMessage("Table name may contain only letters, digits and underscore.");

        RuleFor(x => x.OldColumnName)
            .Cascade(CascadeMode.Stop)
            .NotEmpty().WithMessage("Old column name must be provided.")
            .MaximumLength(MaxIdentifierLength).WithMessage($"Old column name must be at most {MaxIdentifierLength} characters.")
            .Matches(AllowedPattern).WithMessage("Old column name may contain only letters, digits and underscore.");

        RuleFor(x => x.NewColumnName)
            .Cascade(CascadeMode.Stop)
            .NotEmpty().WithMessage("New column name must be provided.")
            .MaximumLength(MaxIdentifierLength).WithMessage($"New column name must be at most {MaxIdentifierLength} characters.")
            .Matches(AllowedPattern).WithMessage("New column name may contain only letters, digits and underscore.");

        RuleFor(x => x)
            .Must(cmd => !string.Equals(cmd.OldColumnName?.Trim(), cmd.NewColumnName?.Trim(), StringComparison.OrdinalIgnoreCase))
            .WithMessage("New column name must be different from the old column name.");
    }
}
