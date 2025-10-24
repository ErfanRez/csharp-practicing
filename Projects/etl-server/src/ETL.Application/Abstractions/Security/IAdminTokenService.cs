namespace ETL.Application.Abstractions.Security;

public interface IAdminTokenService
{
    Task<string?> GetAdminAccessTokenAsync(CancellationToken ct = default);
}