using System.Text.Json;

namespace ETL.Infrastructure.UserServices.Abstractions
{
    public interface IUserFetcher
    {
        Task<List<JsonElement>> FetchAllUsersRawAsync(int? first = null, int? max = null, CancellationToken ct = default);
    }
}

