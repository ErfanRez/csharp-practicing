using System.Text.Json;

namespace ETL.Infrastructure.OAuthClients.Abstractions;

public interface IOAuthGetJsonArray
{
    Task<List<JsonElement>> GetJsonArrayAsync(string relativePath, CancellationToken ct = default);
}
