using ETL.Application.Abstractions.UserServices;
using ETL.Application.Common.Options;
using ETL.Application.User.Edit;
using ETL.Infrastructure.OAuthClients.Abstractions;
using Microsoft.Extensions.Options;

namespace ETL.Infrastructure.UserServices;

public class OAuthUserUpdater : IOAuthUserUpdater
{
    private readonly IOAuthPutJson _putJson;
    private readonly AuthOptions _authOptions;

    public OAuthUserUpdater(IOAuthPutJson putJson, IOptions<AuthOptions> options)
    {
        _putJson = putJson ?? throw new ArgumentNullException(nameof(putJson));
        _authOptions = options?.Value ?? throw new ArgumentNullException(nameof(options));
    }
    public async Task UpdateUserAsync(EditUserCommand command, CancellationToken ct = default)
    {
        var realm = _authOptions.Realm;
        var path = $"/admin/realms/{Uri.EscapeDataString(realm)}/users/{Uri.EscapeDataString(command.UserId)}";

        var payload = new Dictionary<string, object?>();
        if (command.Username is not null) payload["username"] = command.Username;
        if (command.Email is not null) payload["email"] = command.Email;
        if (command.FirstName is not null) payload["firstName"] = command.FirstName;
        if (command.LastName is not null) payload["lastName"] = command.LastName;

        if (payload.Count == 0)
            return;

        await _putJson.PutJsonAsync(path, payload, ct);
    }
}
