using System.Net.Http.Json;
using ETL.Application.Abstractions.Security;
using ETL.Application.Common.DTOs;
using ETL.Application.Common.Options;
using Microsoft.Extensions.Options;

namespace ETL.Infrastructure.Security;

public class AuthTokenRefresher : IAuthTokenRefresher
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly AuthOptions _authOptions;

    public AuthTokenRefresher(IHttpClientFactory httpClientFactory, IOptions<AuthOptions> options)
    {
        _httpClientFactory = httpClientFactory ?? throw new ArgumentNullException(nameof(httpClientFactory));
        _authOptions = options?.Value ?? throw new ArgumentNullException(nameof(options));
    }

    public async Task<TokenResponse> RefreshAsync(string refreshToken, CancellationToken ct = default)
    {
        var httpClient = _httpClientFactory.CreateClient();
        var tokenEndpoint = $"{_authOptions.Authority}/protocol/openid-connect/token";
        var clientId = _authOptions.ClientId;
        var clientSecret = _authOptions.ClientSecret;

        var form = new Dictionary<string, string>
        {
            ["grant_type"] = "refresh_token",
            ["refresh_token"] = refreshToken,
            ["client_id"] = clientId,
            ["client_secret"] = clientSecret
        };

        var response = await httpClient.PostAsync(tokenEndpoint, new FormUrlEncodedContent(form), ct).ConfigureAwait(false);

        if (!response.IsSuccessStatusCode)
        {
            var body = await response.Content.ReadAsStringAsync(ct).ConfigureAwait(false);
            throw new InvalidOperationException($"Failed to refresh token. Status code: {response.StatusCode}, Body: {body}");
        }

        var tokensData = await response.Content.ReadFromJsonAsync<TokenResponse>(cancellationToken: ct).ConfigureAwait(false);

        if (tokensData is null || string.IsNullOrEmpty(tokensData.AccessToken))
            throw new InvalidOperationException("Failed to parse tokens from the response.");

        return tokensData;
    }
}
