using FluentValidation;

namespace ETL.Application.DataSet.DeleteTable;

public sealed class DeleteTableCommandValidator : AbstractValidator<DeleteTableCommand>
{
    private const int MaxIdentifierLength = 63;
    private const string AllowedPattern = @"^\w+$";

    public DeleteTableCommandValidator()
    {
        RuleFor(x => x.TableName)
            .Cascade(CascadeMode.Stop)
            .NotEmpty().WithMessage("Table name must be provided.")
            .MaximumLength(MaxIdentifierLength).WithMessage($"Table name must be at most {MaxIdentifierLength} characters.")
            .Matches(AllowedPattern).WithMessage("Table name may contain only letters, digits and underscore.");
    }
}
