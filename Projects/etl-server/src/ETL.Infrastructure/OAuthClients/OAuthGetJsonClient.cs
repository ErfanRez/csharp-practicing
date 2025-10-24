using System.Text.Json;
using ETL.Application.Abstractions.Security;
using ETL.Application.Common.Options;
using ETL.Infrastructure.OAuthClients.Abstractions;
using Microsoft.Extensions.Options;

namespace ETL.Infrastructure.OAuthClients;

public class OAuthGetJsonClient : OAuthHttpClientBase, IOAuthGetJson
{
    public OAuthGetJsonClient(IHttpClientFactory httpFactory, IAdminTokenService adminTokenService, IOptions<AuthOptions> options)
        : base(httpFactory, adminTokenService, options)
    {
    }

    public async Task<JsonElement> GetJsonAsync(string relativePath, CancellationToken ct = default)
    {
        var token = await GetAdminTokenAsync(ct);

        var url = BuildUrl(relativePath);
        var client = CreateClientWithToken(token);

        using var req = new HttpRequestMessage(HttpMethod.Get, url);
        var resp = await client.SendAsync(req, ct);

        return await ParseResponseJsonAsync(resp, ct);
    }
}
