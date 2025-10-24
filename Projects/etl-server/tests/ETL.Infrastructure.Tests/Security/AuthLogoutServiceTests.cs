using System.Net;
using ETL.Application.Common.Options;
using ETL.Infrastructure.Security;
using ETL.Infrastructure.Tests.HttpClientFixture;
using FluentAssertions;
using Microsoft.Extensions.Options;
using NSubstitute;

namespace ETL.Infrastructure.Tests.Security;

[Collection("HttpClient collection")]
public class AuthLogoutServiceTests
{
    private readonly HttpClientTestFixture _fixture;
    private readonly AuthLogoutService _sut;

    public AuthLogoutServiceTests(HttpClientTestFixture fixture)
    {
        _fixture = fixture;

        var authOptions = new AuthOptions
        {
            Authority = "https://fake-auth",
            ClientId = "client-id",
            ClientSecret = "client-secret",
            RedirectUri = "https://ignored"
        };
        var options = Options.Create(authOptions);

        var httpFactory = Substitute.For<IHttpClientFactory>();
        httpFactory.CreateClient().Returns(_fixture.Client);

        _sut = new AuthLogoutService(httpFactory, options);
    }

    [Fact]
    public void Constructor_ShouldThrowArgumentNullException_WhenHttpClientFactoryIsNull()
    {
        // Arrange // Act
        Action act = () => new AuthLogoutService(null!, Options.Create(new AuthOptions()));

        // Assert
        act.Should().Throw<ArgumentNullException>().WithParameterName("httpClientFactory");
    }

    [Fact]
    public void Constructor_ShouldThrowArgumentNullException_WhenOptionsIsNull()
    {
        // Arrange // Act
        Action act = () => new AuthLogoutService(Substitute.For<IHttpClientFactory>(), null!);

        // Assert
        act.Should().Throw<ArgumentNullException>().WithParameterName("options");
    }

    [Fact]
    public async Task LogoutAsync_ShouldSendRequestWithAuthorization_WhenAccessTokenProvided()
    {
        // Arrange
        _fixture.Handler.SetupResponse(HttpStatusCode.OK, "");

        // Act
        await _sut.LogoutAsync("Bearer fake-access-token", "fake-refresh-token", CancellationToken.None);

        // Assert
        _fixture.Handler.LastRequest!.RequestUri!.ToString().Should().Contain("/protocol/openid-connect/logout");
        _fixture.Handler.LastRequest!.Headers.Authorization!.Parameter.Should().Be("fake-access-token");
    }

    [Fact]
    public async Task LogoutAsync_ShouldThrowInvalidOperationException_WhenResponseFails()
    {
        // Arrange
        _fixture.Handler.SetupResponse(HttpStatusCode.BadRequest, "logout error");

        // Act // Assert
        await FluentActions.Awaiting(() => _sut.LogoutAsync("token", "refresh", CancellationToken.None))
            .Should()
            .ThrowAsync<InvalidOperationException>()
            .WithMessage("*logout error*");
    }

    [Fact]
    public async Task LogoutAsync_ShouldNotSetAuthorizationHeader_WhenAccessTokenIsNull()
    {
        // Arrange
        _fixture.Handler.SetupResponse(HttpStatusCode.OK, "");

        // Act
        await _sut.LogoutAsync(null, "refresh", CancellationToken.None);

        // Assert
        _fixture.Handler.LastRequest!.Headers.Authorization.Should().BeNull();
    }
}
