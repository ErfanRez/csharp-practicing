using System.Text.Json;

namespace ETL.Infrastructure.UserServices.Abstractions
{
    public interface IUsersRoleFetcher
    {
        Task<List<JsonElement>> FetchUsersForRoleAsync(string roleName, CancellationToken ct = default);
    }
}

