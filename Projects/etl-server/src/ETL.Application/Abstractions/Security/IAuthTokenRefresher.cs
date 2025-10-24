using ETL.Application.Common.DTOs;

namespace ETL.Application.Abstractions.Security;

public interface IAuthTokenRefresher
{
    Task<TokenResponse> RefreshAsync(string refreshToken, CancellationToken ct = default);
}
