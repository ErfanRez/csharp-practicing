using System.Text.Json;

namespace ETL.Infrastructure.OAuthClients.Abstractions;

public interface IOAuthGetJson
{
    Task<JsonElement> GetJsonAsync(string relativePath, CancellationToken ct = default);
}
