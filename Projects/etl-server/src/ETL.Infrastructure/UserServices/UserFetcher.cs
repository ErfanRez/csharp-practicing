using System.Text.Json;
using ETL.Application.Common.Options;
using ETL.Infrastructure.OAuthClients.Abstractions;
using ETL.Infrastructure.UserServices.Abstractions;
using Microsoft.Extensions.Options;

namespace ETL.Infrastructure.UserServices
{
    public class UserFetcher : IUserFetcher
    {
        private readonly IOAuthGetJsonArray _getArray;
        private readonly AuthOptions _authOptions;

        public UserFetcher(IOAuthGetJsonArray getArray, IOptions<AuthOptions> options)
        {
            _getArray = getArray ?? throw new ArgumentNullException(nameof(getArray));
            _authOptions = options?.Value ?? throw new ArgumentNullException(nameof(options));
        }

        public async Task<List<JsonElement>> FetchAllUsersRawAsync(int? first = null, int? max = null, CancellationToken ct = default)
        {
            var realm = _authOptions.Realm;
            var path = $"/admin/realms/{Uri.EscapeDataString(realm)}/users";
            var query = new List<string>();
            if (first.HasValue) query.Add($"first={first.Value}");
            if (max.HasValue) query.Add($"max={max.Value}");
            if (query.Count > 0) path = $"{path}?{string.Join('&', query)}";

            var list = await _getArray.GetJsonArrayAsync(path, ct).ConfigureAwait(false);
            return list ?? new List<JsonElement>();
        }
    }
}
