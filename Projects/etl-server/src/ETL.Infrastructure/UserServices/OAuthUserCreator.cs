using ETL.Application.Abstractions.UserServices;
using ETL.Application.Common.Options;
using ETL.Application.User.Create;
using ETL.Infrastructure.OAuthClients.Abstractions;
using Microsoft.Extensions.Options;

namespace ETL.Infrastructure.UserServices;

public class OAuthUserCreator : IOAuthUserCreator
{
    private readonly IOAuthPostJsonWithResponse _postWithResponse;
    private readonly AuthOptions _authOptions;

    public OAuthUserCreator(IOAuthPostJsonWithResponse postWithResponse, IOptions<AuthOptions> options)
    {
        _postWithResponse = postWithResponse ?? throw new ArgumentNullException(nameof(postWithResponse));
        _authOptions = options?.Value ?? throw new ArgumentNullException(nameof(options));
    }

    public async Task<string> CreateUserAsync(CreateUserCommand command, CancellationToken ct = default)
    {
        var realm = _authOptions.Realm;
        var path = $"/admin/realms/{Uri.EscapeDataString(realm)}/users";

        var newUserPayload = new
        {
            username = command.Username,
            email = command.Email,
            firstName = command.FirstName,
            lastName = command.LastName,
            enabled = true,
            credentials = new[]
            {
                new { type = "password", value = command.Password, temporary = false }
            }
        };

        var resp = await _postWithResponse.PostJsonForResponseAsync(path, newUserPayload, ct);

        if (!resp.IsSuccessStatusCode)
        {
            var body = await resp.Content.ReadAsStringAsync(ct);
            throw new HttpRequestException($"Create user failed: {resp.StatusCode} - {body}", null, resp.StatusCode);
        }

        var location = resp.Headers.Location ?? throw new InvalidOperationException("Provider did not return Location header for created user.");
        string newUserId;
        var segments = location.AbsolutePath.Split('/', StringSplitOptions.RemoveEmptyEntries);
        newUserId = segments.Last();
        if (string.IsNullOrWhiteSpace(newUserId))
            throw new InvalidOperationException("Failed to parse created user id from Location header.");

        return newUserId;
    }
}
