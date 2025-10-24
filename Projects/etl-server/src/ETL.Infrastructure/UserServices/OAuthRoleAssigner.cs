using ETL.Application.Abstractions.UserServices;
using ETL.Application.Common.Options;
using ETL.Infrastructure.OAuthClients.Abstractions;
using Microsoft.Extensions.Options;

namespace ETL.Infrastructure.UserServices;

public class OAuthRoleAssigner : IOAuthRoleAssigner
{
    private readonly IOAuthGetJson _getJson;
    private readonly IOAuthPostJson _postJson;
    private readonly AuthOptions _authOptions;

    public OAuthRoleAssigner(IOAuthGetJson getJson, IOAuthPostJson postJson, IOptions<AuthOptions> options)
    {
        _getJson = getJson ?? throw new ArgumentNullException(nameof(getJson));
        _postJson = postJson ?? throw new ArgumentNullException(nameof(postJson));
        _authOptions = options?.Value ?? throw new ArgumentNullException(nameof(options));
    }
    public async Task AssignRoleAsync(string userId, string roleName, CancellationToken ct = default)
    {
        if (string.IsNullOrWhiteSpace(roleName))
            return;

        var realm = _authOptions.Realm;

        var getRolePath = $"/admin/realms/{Uri.EscapeDataString(realm)}/roles/{Uri.EscapeDataString(roleName)}";
        var roleDef = await _getJson.GetJsonAsync(getRolePath, ct);

        var assignPath = $"/admin/realms/{Uri.EscapeDataString(realm)}/users/{Uri.EscapeDataString(userId)}/role-mappings/realm";

        await _postJson.PostJsonAsync(assignPath, new[] { roleDef }, ct);
    }
}
