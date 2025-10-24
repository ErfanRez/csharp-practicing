namespace ETL.Application.Abstractions.Security;

public interface IAuthRestPasswordService
{
    Task ResetPasswordAsync(string userId, string newPassword, CancellationToken ct = default);
}
