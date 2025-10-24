using FluentValidation;

namespace ETL.Application.DataSet.UploadFile;

public sealed class UploadCsvCommandValidator : AbstractValidator<UploadCsvCommand>
{
    private const int MaxIdentifierLength = 63;
    private const string AllowedPattern = @"^\w+$";

    public UploadCsvCommandValidator()
    {
        RuleFor(x => x.TableName)
            .Cascade(CascadeMode.Stop)
            .NotEmpty().WithMessage("Table name must be provided.")
            .MaximumLength(MaxIdentifierLength).WithMessage($"Table name must be at most {MaxIdentifierLength} characters.")
            .Matches(AllowedPattern).WithMessage("Table name may contain only letters, digits and underscore.");

        RuleFor(x => x.FileStream)
            .Cascade(CascadeMode.Stop)
            .NotNull().WithMessage("CSV file must be provided.")
            .Must(s => s!.CanRead).WithMessage("Provided file stream must be readable.")
            .Must(s => !s!.CanSeek || s.Length > 0).WithMessage("Provided file stream is empty.");

        RuleFor(x => x.UserId)
            .NotEmpty().WithMessage("UserId must be provided.");
    }
}
