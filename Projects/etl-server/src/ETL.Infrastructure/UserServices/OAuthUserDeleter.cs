using ETL.Application.Abstractions.UserServices;
using ETL.Application.Common.Options;
using ETL.Infrastructure.OAuthClients.Abstractions;
using Microsoft.Extensions.Options;

namespace ETL.Infrastructure.UserServices;

public class OAuthUserDeleter : IOAuthUserDeleter
{
    private readonly IOAuthGetJson _getJson;
    private readonly IOAuthDeleteJson _delete;
    private readonly AuthOptions _authOptions;

    public OAuthUserDeleter(IOAuthGetJson getJson, IOAuthDeleteJson delete, IOptions<AuthOptions> options)
    {
        _getJson = getJson ?? throw new ArgumentNullException(nameof(getJson));
        _delete = delete ?? throw new ArgumentNullException(nameof(delete));
        _authOptions = options?.Value ?? throw new ArgumentNullException(nameof(options));
    }

    public async Task DeleteUserAsync(string userId, CancellationToken ct = default)
    {
        var realm = _authOptions.Realm;
        var path = $"/admin/realms/{Uri.EscapeDataString(realm)}/users/{Uri.EscapeDataString(userId)}";

        await _getJson.GetJsonAsync(path, ct);

        await _delete.DeleteJsonAsync(path, null, ct);
    }
}
