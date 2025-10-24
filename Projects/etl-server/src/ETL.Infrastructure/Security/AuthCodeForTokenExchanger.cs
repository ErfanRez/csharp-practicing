using System.Net.Http.Json;
using ETL.Application.Abstractions.Security;
using ETL.Application.Common.DTOs;
using ETL.Application.Common.Options;
using Microsoft.Extensions.Options;

namespace ETL.Infrastructure.Security;

public class AuthCodeForTokenExchanger : IAuthCodeForTokenExchanger
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly AuthOptions _authOptions;

    public AuthCodeForTokenExchanger(IHttpClientFactory httpClientFactory, IOptions<AuthOptions> options)
    {
        _httpClientFactory = httpClientFactory ?? throw new ArgumentNullException(nameof(httpClientFactory));
        _authOptions = options?.Value ?? throw new ArgumentNullException(nameof(options));
    }

    public async Task<TokenResponse> ExchangeCodeForTokensAsync(string code, string redirectPath, CancellationToken ct = default)
    {
        var httpClient = _httpClientFactory.CreateClient();
        var tokenEndpoint = $"{_authOptions.Authority}/protocol/openid-connect/token";
        var clientId = _authOptions.ClientId;
        var clientSecret = _authOptions.ClientSecret;
        var redirectUri = $"{_authOptions.RedirectUri}/{redirectPath}";

        var requestBody = new Dictionary<string, string>
        {
            { "grant_type", "authorization_code" },
            { "code", code },
            { "redirect_uri", redirectUri },
            { "client_id", clientId },
            { "client_secret", clientSecret }
        };

        var response = await httpClient.PostAsync(tokenEndpoint, new FormUrlEncodedContent(requestBody), ct);

        if (!response.IsSuccessStatusCode)
        {
            var err = await response.Content.ReadAsStringAsync(ct);
            throw new InvalidOperationException($"Token exchange failed: {err}");
        }

        var tokensData = await response.Content.ReadFromJsonAsync<TokenResponse>(cancellationToken: ct);

        return tokensData is null ? throw new InvalidOperationException("Failed to parse tokens from the response.") : tokensData;
    }
}
