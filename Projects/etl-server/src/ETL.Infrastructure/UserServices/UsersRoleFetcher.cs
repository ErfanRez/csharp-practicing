using System.Text.Json;
using ETL.Application.Common.Options;
using ETL.Infrastructure.OAuthClients.Abstractions;
using ETL.Infrastructure.UserServices.Abstractions;
using Microsoft.Extensions.Options;

namespace ETL.Infrastructure.UserServices
{
    public class UsersRoleFetcher : IUsersRoleFetcher
    {
        private readonly IOAuthGetJsonArray _getArray;
        private readonly AuthOptions _authOptions;

        public UsersRoleFetcher(IOAuthGetJsonArray getArray, IOptions<AuthOptions> options)
        {
            _getArray = getArray ?? throw new ArgumentNullException(nameof(getArray));
            _authOptions = options?.Value ?? throw new ArgumentNullException(nameof(options));
        }

        public async Task<List<JsonElement>> FetchUsersForRoleAsync(string roleName, CancellationToken ct = default)
        {
            if (string.IsNullOrWhiteSpace(roleName))
                return new List<JsonElement>();

            var realm = _authOptions.Realm;
            var encodedRole = Uri.EscapeDataString(roleName);
            var path = $"/admin/realms/{Uri.EscapeDataString(realm)}/roles/{encodedRole}/users";

            var list = await _getArray.GetJsonArrayAsync(path, ct).ConfigureAwait(false);
            return list ?? new List<JsonElement>();
        }
    }
}
