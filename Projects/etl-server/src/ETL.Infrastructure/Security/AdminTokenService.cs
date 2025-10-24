using System.Net.Http.Json;
using ETL.Application.Abstractions.Security;
using ETL.Application.Common.DTOs;
using ETL.Application.Common.Options;
using Microsoft.Extensions.Options;

namespace ETL.Infrastructure.Security;

public class AdminTokenService : IAdminTokenService
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly AuthOptions _authOptions;
    private readonly OAuthAdminOptions _adminOptions;

    public AdminTokenService(IHttpClientFactory httpClientFactory, IOptions<AuthOptions> AuthOptions, IOptions<OAuthAdminOptions> adminOptions)
    {
        _httpClientFactory = httpClientFactory ?? throw new ArgumentNullException(nameof(httpClientFactory));
        _authOptions = AuthOptions?.Value ?? throw new ArgumentNullException(nameof(AuthOptions));
        _adminOptions = adminOptions?.Value ?? throw new ArgumentNullException(nameof(adminOptions));
    }

    public async Task<string?> GetAdminAccessTokenAsync(CancellationToken ct = default)
    {
        var httpClient = _httpClientFactory.CreateClient();
        var tokenEndpoint = $"{_authOptions.Authority}/protocol/openid-connect/token";
        var adminClientId = _adminOptions.ClientId;
        var adminClientSecret = _adminOptions.ClientSecret;

        var adminTokenBody = new Dictionary<string, string>
        {
            ["grant_type"] = "client_credentials",
            ["client_id"] = adminClientId,
            ["client_secret"] = adminClientSecret
        };

        var response = await httpClient.PostAsync(tokenEndpoint, new FormUrlEncodedContent(adminTokenBody), ct);

        if (!response.IsSuccessStatusCode)
        {
            var err = await response.Content.ReadAsStringAsync(ct);
            throw new InvalidOperationException($"Failed to obtain admin access token from OAuth. {err}");
        }

        var adminTokenData = await response.Content.ReadFromJsonAsync<TokenResponse>(cancellationToken: ct);
        return adminTokenData?.AccessToken;
    }
}
