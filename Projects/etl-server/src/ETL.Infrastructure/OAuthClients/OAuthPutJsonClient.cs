using System.Net;
using System.Net.Http.Json;
using ETL.Application.Abstractions.Security;
using ETL.Application.Common.Options;
using ETL.Infrastructure.OAuthClients.Abstractions;
using Microsoft.Extensions.Options;

namespace ETL.Infrastructure.OAuthClients
{
    public class OAuthPutJsonClient : OAuthHttpClientBase, IOAuthPutJson
    {
        public OAuthPutJsonClient(IHttpClientFactory httpFactory, IAdminTokenService adminTokenService, IOptions<AuthOptions> options)
            : base(httpFactory, adminTokenService, options)
        {
        }

        public async Task PutJsonAsync(string relativePath, object content, CancellationToken ct = default)
        {
            var token = await GetAdminTokenAsync(ct);

            var url = BuildUrl(relativePath);
            var client = CreateClientWithToken(token);

            using var req = new HttpRequestMessage(HttpMethod.Put, url)
            {
                Content = JsonContent.Create(content)
            };

            var resp = await client.SendAsync(req, ct);
            if (!resp.IsSuccessStatusCode)
            {
                var body = await resp.Content.ReadAsStringAsync(ct);
                if (resp.StatusCode == HttpStatusCode.NotFound)
                    throw new HttpRequestException($"Resource not found: {url}", null, resp.StatusCode);

                throw new HttpRequestException($"PUT {url} failed: {resp.StatusCode} - {body}", null, resp.StatusCode);
            }
        }
    }
}
