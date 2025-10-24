using System.Net;
using System.Text.Json;
using ETL.Application.Abstractions.Security;
using ETL.Application.Common.Options;
using ETL.Infrastructure.OAuthClients.Abstractions;
using Microsoft.Extensions.Options;

namespace ETL.Infrastructure.OAuthClients;

public class OAuthGetJsonArrayClient : OAuthHttpClientBase, IOAuthGetJsonArray
{
    public OAuthGetJsonArrayClient(IHttpClientFactory httpFactory, IAdminTokenService adminTokenService, IOptions<AuthOptions> options)
        : base(httpFactory, adminTokenService, options)
    {
    }

    public async Task<List<JsonElement>> GetJsonArrayAsync(string relativePath, CancellationToken ct = default)
    {
        var token = await GetAdminTokenAsync(ct);

        var url = BuildUrl(relativePath);
        var client = CreateClientWithToken(token);

        using var req = new HttpRequestMessage(HttpMethod.Get, url);
        var resp = await client.SendAsync(req, ct);

        if (!resp.IsSuccessStatusCode)
        {
            var body = await resp.Content.ReadAsStringAsync(ct);
            if (resp.StatusCode == HttpStatusCode.NotFound)
                throw new HttpRequestException($"Resource not found: {url}", null, resp.StatusCode);

            throw new HttpRequestException($"GET {url} failed: {resp.StatusCode} - {body}", null, resp.StatusCode);
        }

        await using var stream = await resp.Content.ReadAsStreamAsync(ct);
        using var doc = await JsonDocument.ParseAsync(stream, cancellationToken: ct);

        var list = new List<JsonElement>();
        if (doc.RootElement.ValueKind == JsonValueKind.Array)
        {
            foreach (var el in doc.RootElement.EnumerateArray())
                list.Add(el.Clone());
        }

        return list;
    }
}
