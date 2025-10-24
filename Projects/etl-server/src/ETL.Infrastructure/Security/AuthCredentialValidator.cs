using ETL.Application.Abstractions.Security;
using ETL.Application.Common.Options;
using Microsoft.Extensions.Options;

namespace ETL.Infrastructure.Security;

public class AuthCredentialValidator : IAuthCredentialValidator
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly AuthOptions _authOptions;

    public AuthCredentialValidator(IHttpClientFactory httpClientFactory, IOptions<AuthOptions> options)
    {
        _httpClientFactory = httpClientFactory ?? throw new ArgumentNullException(nameof(httpClientFactory));
        _authOptions = options?.Value ?? throw new ArgumentNullException(nameof(options));
    }

    public async Task<bool> ValidateCredentialsAsync(string username, string password, CancellationToken ct = default)
    {
        var httpClient = _httpClientFactory.CreateClient();
        var tokenEndpoint = $"{_authOptions.Authority}/protocol/openid-connect/token";
        var clientId = _authOptions.ClientId;
        var clientSecret = _authOptions.ClientSecret;

        var body = new Dictionary<string, string>
        {
            ["grant_type"] = "password",
            ["client_id"] = clientId,
            ["client_secret"] = clientSecret,
            ["username"] = username,
            ["password"] = password
        };

        var response = await httpClient.PostAsync(tokenEndpoint, new FormUrlEncodedContent(body), ct);

        return response.IsSuccessStatusCode;
    }
}
