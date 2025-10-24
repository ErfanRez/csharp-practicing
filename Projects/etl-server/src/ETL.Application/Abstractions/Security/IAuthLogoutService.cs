namespace ETL.Application.Abstractions.Security;
public interface IAuthLogoutService
{
    Task LogoutAsync(string accessToken, string refreshToken, CancellationToken ct = default);
}