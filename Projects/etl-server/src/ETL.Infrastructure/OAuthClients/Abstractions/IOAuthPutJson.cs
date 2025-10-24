namespace ETL.Infrastructure.OAuthClients.Abstractions;

public interface IOAuthPutJson
{
    Task PutJsonAsync(string relativePath, object content, CancellationToken ct = default);
}
