namespace ETL.Application.Abstractions.Security;

public interface IAuthCredentialValidator
{
    Task<bool> ValidateCredentialsAsync(string username, string password, CancellationToken ct = default);
}
