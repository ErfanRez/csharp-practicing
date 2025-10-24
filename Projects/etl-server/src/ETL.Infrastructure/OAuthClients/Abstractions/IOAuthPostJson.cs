namespace ETL.Infrastructure.OAuthClients.Abstractions;

public interface IOAuthPostJson
{
    Task PostJsonAsync(string relativePath, object content, CancellationToken ct = default);
}
