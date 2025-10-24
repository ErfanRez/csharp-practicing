using System.Diagnostics.CodeAnalysis;
using System.Text.Json;
using ETL.Application.Abstractions.Security;
using ETL.Application.Common.Options;
using Microsoft.Extensions.Options;

namespace ETL.Infrastructure.OAuthClients;

[ExcludeFromCodeCoverage]
public abstract class OAuthHttpClientBase
{
    private readonly IHttpClientFactory _httpFactory;
    private readonly IAdminTokenService _adminTokenService;
    private readonly AuthOptions _authOptions;

    protected OAuthHttpClientBase(IHttpClientFactory httpFactory, IAdminTokenService adminTokenService, IOptions<AuthOptions> options)
    {
        _httpFactory = httpFactory ?? throw new ArgumentNullException(nameof(httpFactory));
        _adminTokenService = adminTokenService ?? throw new ArgumentNullException(nameof(adminTokenService));
        _authOptions = options?.Value ?? throw new ArgumentNullException(nameof(options));
    }

    protected string BuildUrl(string relativePath)
    {
        var baseUrl = _authOptions.BaseUrl.TrimEnd('/');

        if (string.IsNullOrWhiteSpace(relativePath))
            throw new ArgumentException("relativePath is required", nameof(relativePath));

        if (relativePath.StartsWith("/"))
            return $"{baseUrl}{relativePath}";

        return $"{baseUrl}/{relativePath}";
    }

    protected HttpClient CreateClientWithToken(string token)
    {
        var client = _httpFactory.CreateClient();
        client.DefaultRequestHeaders.Authorization =
            new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
        return client;
    }

    protected async Task<string> GetAdminTokenAsync(CancellationToken ct = default)
    {
        var token = await _adminTokenService.GetAdminAccessTokenAsync(ct);
        if (string.IsNullOrEmpty(token))
            throw new InvalidOperationException("Could not obtain admin token");
        return token;
    }
    protected async Task<JsonElement> ParseResponseJsonAsync(HttpResponseMessage resp, CancellationToken ct)
    {
        if (!resp.IsSuccessStatusCode)
        {
            var body = await resp.Content.ReadAsStringAsync(ct);
            throw new HttpRequestException($"Request failed: {resp.StatusCode} - {body}", null, resp.StatusCode);
        }

        await using var stream = await resp.Content.ReadAsStreamAsync(ct);
        using var doc = await JsonDocument.ParseAsync(stream, cancellationToken: ct);
        return doc.RootElement.Clone();
    }
}
