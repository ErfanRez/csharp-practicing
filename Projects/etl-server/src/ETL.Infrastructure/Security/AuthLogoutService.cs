using ETL.Application.Abstractions.Security;
using ETL.Application.Common.Options;
using Microsoft.Extensions.Options;

namespace ETL.Infrastructure.Security;

public class AuthLogoutService : IAuthLogoutService
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly AuthOptions _authOptions;

    public AuthLogoutService(IHttpClientFactory httpClientFactory, IOptions<AuthOptions> options)
    {
        _httpClientFactory = httpClientFactory ?? throw new ArgumentNullException(nameof(httpClientFactory));
        _authOptions = options?.Value ?? throw new ArgumentNullException(nameof(options));
    }

    public async Task LogoutAsync(string? accessToken, string? refreshToken, CancellationToken ct = default)
    {
        var httpClient = _httpClientFactory.CreateClient();
        var logoutEndpoint = $"{_authOptions.Authority}/protocol/openid-connect/logout";
        var clientId = _authOptions.ClientId;
        var clientSecret = _authOptions.ClientSecret;

        var logoutData = new Dictionary<string, string>
        {
            { "client_id", clientId },
            { "client_secret", clientSecret },
            { "refresh_token", refreshToken ?? string.Empty }
        };

        var request = new HttpRequestMessage(HttpMethod.Post, logoutEndpoint);
        request.Content = new FormUrlEncodedContent(logoutData);

        if (!string.IsNullOrEmpty(accessToken))
            request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", accessToken.StartsWith("Bearer ") ? accessToken.Substring(7) : accessToken);

        var response = await httpClient.SendAsync(request, ct);

        if (!response.IsSuccessStatusCode)
        {
            var err = await response.Content.ReadAsStringAsync(ct);
            throw new InvalidOperationException($"Logout failed: {err}");
        }
    }
}
