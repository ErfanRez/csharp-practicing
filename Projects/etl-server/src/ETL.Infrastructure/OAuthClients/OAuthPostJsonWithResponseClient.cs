using System.Net.Http.Json;
using ETL.Application.Abstractions.Security;
using ETL.Application.Common.Options;
using ETL.Infrastructure.OAuthClients.Abstractions;
using Microsoft.Extensions.Options;

namespace ETL.Infrastructure.OAuthClients;

public class OAuthPostJsonWithResponseClient : OAuthHttpClientBase, IOAuthPostJsonWithResponse
{
    public OAuthPostJsonWithResponseClient(IHttpClientFactory httpFactory, IAdminTokenService adminTokenService, IOptions<AuthOptions> options)
        : base(httpFactory, adminTokenService, options)
    {
    }

    public async Task<HttpResponseMessage> PostJsonForResponseAsync(string relativePath, object content, CancellationToken ct = default)
    {
        var token = await GetAdminTokenAsync(ct);

        var url = BuildUrl(relativePath);
        var client = CreateClientWithToken(token);

        var req = new HttpRequestMessage(HttpMethod.Post, url)
        {
            Content = JsonContent.Create(content)
        };

        try
        {
            var resp = await client.SendAsync(req, ct).ConfigureAwait(false);
            return resp;
        }
        catch (Exception ex)
        {
            throw new HttpRequestException($"HTTP request failed: {ex.Message}", ex);
        }
    }
}
