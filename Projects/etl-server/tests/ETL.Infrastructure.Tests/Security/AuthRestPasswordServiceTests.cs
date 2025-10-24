using System.Net;
using System.Text.Json;
using ETL.Application.Abstractions.Security;
using ETL.Application.Common.Options;
using ETL.Infrastructure.Security;
using ETL.Infrastructure.Tests.HttpClientFixture;
using FluentAssertions;
using Microsoft.Extensions.Options;
using NSubstitute;

namespace ETL.Infrastructure.Tests.Security;

[Collection("HttpClient collection")]
public class AuthRestPasswordServiceTests
{
    private readonly HttpClientTestFixture _fixture;
    private readonly IAdminTokenService _adminTokenService;
    private readonly AuthRestPasswordService _sut;

    public AuthRestPasswordServiceTests(HttpClientTestFixture fixture)
    {
        _fixture = fixture;

        _adminTokenService = Substitute.For<IAdminTokenService>();
        _adminTokenService.GetAdminAccessTokenAsync(Arg.Any<CancellationToken>()).Returns("fake-admin-token");

        var authOptions = new AuthOptions
        {
            BaseUrl = "https://fake.keycloak",
            Realm = "fake-realm"
        };
        var options = Options.Create(authOptions);

        var httpFactory = Substitute.For<IHttpClientFactory>();
        httpFactory.CreateClient().Returns(_fixture.Client);

        _sut = new AuthRestPasswordService(httpFactory, _adminTokenService, options);
    }

    [Fact]
    public void Constructor_ShouldThrowArgumentNullException_WhenHttpClientFactoryIsNull()
    {
        // Arrange // Act
        Action act = () => new AuthRestPasswordService(null!, _adminTokenService, Options.Create(new AuthOptions()));

        // Assert
        act.Should().Throw<ArgumentNullException>().WithParameterName("httpClientFactory");
    }

    [Fact]
    public void Constructor_ShouldThrowArgumentNullException_WhenAdminTokenServiceIsNull()
    {
        // Arrange // Act
        Action act = () => new AuthRestPasswordService(Substitute.For<IHttpClientFactory>(), null!, Options.Create(new AuthOptions()));

        // Assert
        act.Should().Throw<ArgumentNullException>().WithParameterName("adminTokenService");
    }

    [Fact]
    public void Constructor_ShouldThrowArgumentNullException_WhenOptionsIsNull()
    {
        // Arrange // Act
        Action act = () => new AuthRestPasswordService(Substitute.For<IHttpClientFactory>(), _adminTokenService, null!);

        // Assert
        act.Should().Throw<ArgumentNullException>().WithParameterName("options");
    }

    [Fact]
    public async Task ResetPasswordAsync_ShouldThrow_WhenAdminTokenIsEmpty()
    {
        // Arrange
        _adminTokenService.GetAdminAccessTokenAsync(Arg.Any<CancellationToken>()).Returns(string.Empty);

        // Act
        Func<Task> act = () => _sut.ResetPasswordAsync("user123", "newPassword", CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<InvalidOperationException>().WithMessage("*Could not obtain admin credentials*");
    }

    [Fact]
    public async Task ResetPasswordAsync_ShouldSendRequestWithAuthorization_WhenResponseIsSuccessful()
    {
        // Arrange
        _fixture.Handler.SetupResponse(HttpStatusCode.NoContent, "");

        // Act
        await _sut.ResetPasswordAsync("user123", "newPassword", CancellationToken.None);

        // Assert
        _fixture.Handler.LastRequest!.Method.Should().Be(HttpMethod.Put);
        _fixture.Handler.LastRequest!.Headers.Authorization!.Parameter.Should().Be("fake-admin-token");
    }

    [Fact]
    public async Task ResetPasswordAsync_ShouldThrowInvalidOperationException_WhenResponseFails()
    {
        // Arrange
        _fixture.Handler.SetupResponse(HttpStatusCode.BadRequest, "reset error");

        // Act
        Func<Task> act = () => _sut.ResetPasswordAsync("user123", "newPassword", CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<InvalidOperationException>().WithMessage("*reset error*");
    }

    [Fact]
    public async Task ResetPasswordAsync_ShouldSendCorrectPayload_WhenCalled()
    {
        // Arrange
        _fixture.Handler.SetupResponse(HttpStatusCode.NoContent, "");
        var userId = "user123";
        var newPassword = "myNewSecret123";

        // Act
        await _sut.ResetPasswordAsync(userId, newPassword, CancellationToken.None);

        // Assert
        var body = await _fixture.Handler.LastRequest!.Content!.ReadAsStringAsync();
        using var doc = JsonDocument.Parse(body);
        var actual = new
        {
            type = doc.RootElement.GetProperty("type").GetString(),
            temporary = doc.RootElement.GetProperty("temporary").GetBoolean(),
            value = doc.RootElement.GetProperty("value").GetString()
        };
        var expected = new { type = "password", temporary = false, value = newPassword };
        actual.Should().BeEquivalentTo(expected);
    }
}
