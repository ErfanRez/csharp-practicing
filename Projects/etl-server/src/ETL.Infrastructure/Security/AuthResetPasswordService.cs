using System.Text;
using System.Text.Json;
using ETL.Application.Abstractions.Security;
using ETL.Application.Common.Options;
using Microsoft.Extensions.Options;

namespace ETL.Infrastructure.Security;

public class AuthRestPasswordService : IAuthRestPasswordService
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly IAdminTokenService _adminTokenService;
    private readonly AuthOptions _authOptions;

    public AuthRestPasswordService(IHttpClientFactory httpClientFactory, IAdminTokenService adminTokenService, IOptions<AuthOptions> options)
    {
        _httpClientFactory = httpClientFactory ?? throw new ArgumentNullException(nameof(httpClientFactory));
        _adminTokenService = adminTokenService ?? throw new ArgumentNullException(nameof(adminTokenService));
        _authOptions = options?.Value ?? throw new ArgumentNullException(nameof(options));
    }

    public async Task ResetPasswordAsync(string userId, string newPassword, CancellationToken ct = default)
    {
        var adminAccessToken = await _adminTokenService.GetAdminAccessTokenAsync(ct);
        if (string.IsNullOrEmpty(adminAccessToken))
            throw new InvalidOperationException("Could not obtain admin credentials.");

        var httpClient = _httpClientFactory.CreateClient();
        var OAuthBaseUrl = _authOptions.BaseUrl;
        var realm = _authOptions.Realm;

        var resetPasswordUrl = $"{OAuthBaseUrl}/admin/realms/{realm}/users/{userId}/reset-password";

        var resetPasswordPayload = new { type = "password", temporary = false, value = newPassword };

        var jsonContent = new StringContent(JsonSerializer.Serialize(resetPasswordPayload), Encoding.UTF8, "application/json");
        var request = new HttpRequestMessage(HttpMethod.Put, resetPasswordUrl);
        request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", adminAccessToken);
        request.Content = jsonContent;

        var response = await httpClient.SendAsync(request, ct);
        if (!response.IsSuccessStatusCode)
        {
            var err = await response.Content.ReadAsStringAsync(ct);
            throw new InvalidOperationException($"Reset password failed: {err}");
        }
    }
}
