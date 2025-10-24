namespace ETL.Infrastructure.OAuthClients.Abstractions;

public interface IOAuthPostJsonWithResponse
{
    Task<HttpResponseMessage> PostJsonForResponseAsync(string relativePath, object content, CancellationToken ct = default);
}
