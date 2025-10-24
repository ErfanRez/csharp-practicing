using System.Text.Json;
using ETL.Application.Abstractions.UserServices;
using ETL.Application.Common.Options;
using ETL.Infrastructure.OAuthClients.Abstractions;
using Microsoft.Extensions.Options;

namespace ETL.Infrastructure.UserServices;

public class OAuthUserRoleGetter : IOAuthUserRoleGetter
{
    private readonly IOAuthGetJsonArray _getArray;
    private readonly AuthOptions _authOptions;

    public OAuthUserRoleGetter(IOAuthGetJsonArray getArray, IOptions<AuthOptions> options)
    {
        _getArray = getArray ?? throw new ArgumentNullException(nameof(getArray));
        _authOptions = options?.Value ?? throw new ArgumentNullException(nameof(options));
    }

    public async Task<string?> GetRoleForUserAsync(string userId, CancellationToken ct = default)
    {
        var realm = _authOptions.Realm;
        var path = $"/admin/realms/{Uri.EscapeDataString(realm)}/users/{Uri.EscapeDataString(userId)}/role-mappings/realm";

        List<JsonElement> arr = await _getArray.GetJsonArrayAsync(path, ct) ?? new List<JsonElement>();

        if (arr.Count == 0) return null;

        foreach (var el in arr)
        {
            if (el.TryGetProperty("name", out var nameProp))
            {
                var rn = nameProp.GetString();
                if (!string.IsNullOrEmpty(rn))
                    return rn;
            }
        }

        return null;
    }
}
