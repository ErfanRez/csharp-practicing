using FluentValidation;

namespace ETL.Application.DataSet.DeleteColumn;

public sealed class DeleteColumnCommandValidator : AbstractValidator<DeleteColumnCommand>
{
    private const int MaxIdentifierLength = 63;
    private const string AllowedPattern = @"^\w+$";

    public DeleteColumnCommandValidator()
    {
        RuleFor(x => x.TableName)
            .Cascade(CascadeMode.Stop)
            .NotEmpty().WithMessage("Table name must be provided.")
            .MaximumLength(MaxIdentifierLength).WithMessage($"Table name must be at most {MaxIdentifierLength} characters.")
            .Matches(AllowedPattern).WithMessage("Table name may contain only letters, digits and underscore.");

        RuleFor(x => x.ColumnName)
            .Cascade(CascadeMode.Stop)
            .NotEmpty().WithMessage("Column name must be provided.")
            .MaximumLength(MaxIdentifierLength).WithMessage($"Column name must be at most {MaxIdentifierLength} characters.")
            .Matches(AllowedPattern).WithMessage("Column name may contain only letters, digits and underscore.");
    }
}
