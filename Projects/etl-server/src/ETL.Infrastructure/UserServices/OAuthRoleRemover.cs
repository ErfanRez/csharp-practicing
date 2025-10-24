using System.Text.Json;
using ETL.Application.Abstractions.UserServices;
using ETL.Application.Common.Options;
using ETL.Infrastructure.OAuthClients.Abstractions;
using Microsoft.Extensions.Options;

namespace ETL.Infrastructure.UserServices;

public class OAuthRoleRemover : IOAuthRoleRemover
{
    private readonly IOAuthGetJsonArray _getArray;
    private readonly IOAuthDeleteJson _delete;
    private readonly AuthOptions _authOptions;

    public OAuthRoleRemover(IOAuthGetJsonArray getArray, IOAuthDeleteJson delete, IOptions<AuthOptions> options)
    {
        _getArray = getArray ?? throw new ArgumentNullException(nameof(getArray));
        _delete = delete ?? throw new ArgumentNullException(nameof(delete));
        _authOptions = options?.Value ?? throw new ArgumentNullException(nameof(options));
    }

    public async Task RemoveAllRealmRolesAsync(string userId, CancellationToken ct = default)
    {
        var realm = _authOptions.Realm;
        var rolesPath = $"/admin/realms/{Uri.EscapeDataString(realm)}/users/{Uri.EscapeDataString(userId)}/role-mappings/realm";

        var roles = await _getArray.GetJsonArrayAsync(rolesPath, ct) ?? new List<JsonElement>();

        if (roles.Count == 0) return;

        await _delete.DeleteJsonAsync(rolesPath, roles, ct);
    }
}
