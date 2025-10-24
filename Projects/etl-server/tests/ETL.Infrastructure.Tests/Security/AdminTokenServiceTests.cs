using System.Net;
using System.Text.Json;
using ETL.Application.Common.DTOs;
using ETL.Application.Common.Options;
using ETL.Infrastructure.Security;
using ETL.Infrastructure.Tests.HttpClientFixture;
using FluentAssertions;
using Microsoft.Extensions.Options;
using NSubstitute;

namespace ETL.Infrastructure.Tests.Security;

[Collection("HttpClient collection")]
public class AdminTokenServiceTests
{
    private readonly HttpClientTestFixture _fixture;
    private readonly AdminTokenService _sut;

    public AdminTokenServiceTests(HttpClientTestFixture fixture)
    {
        _fixture = fixture;

        var authOptions = new AuthOptions
        {
            Authority = "https://fake-auth"
        };
        var adminOptions = new OAuthAdminOptions
        {
            ClientId = "client-id",
            ClientSecret = "client-secret"
        };

        var authOptionsWrap = Options.Create(authOptions);
        var adminOptionsWrap = Options.Create(adminOptions);

        var httpFactory = Substitute.For<IHttpClientFactory>();
        httpFactory.CreateClient().Returns(_fixture.Client);

        _sut = new AdminTokenService(httpFactory, authOptionsWrap, adminOptionsWrap);
    }

    [Fact]
    public void Constructor_ShouldThrowArgumentNullException_WhenHttpClientFactoryIsNull()
    {
        // Arrange // Act
        Action act = () => new AdminTokenService(null!, Options.Create(new AuthOptions()), Options.Create(new OAuthAdminOptions()));

        // Assert
        act.Should().Throw<ArgumentNullException>().WithParameterName("httpClientFactory");
    }

    [Fact]
    public void Constructor_ShouldThrowArgumentNullException_WhenAuthOptionsIsNull()
    {
        // Arrange // Act
        Action act = () => new AdminTokenService(Substitute.For<IHttpClientFactory>(), null!, Options.Create(new OAuthAdminOptions()));

        // Assert
        act.Should().Throw<ArgumentNullException>().WithParameterName("AuthOptions");
    }

    [Fact]
    public void Constructor_ShouldThrowArgumentNullException_WhenAdminOptionsIsNull()
    {
        // Arrange // Act
        Action act = () => new AdminTokenService(Substitute.For<IHttpClientFactory>(), Options.Create(new AuthOptions()), null!);

        // Assert
        act.Should().Throw<ArgumentNullException>().WithParameterName("adminOptions");
    }

    [Fact]
    public async Task GetAdminAccessTokenAsync_ShouldReturnToken_WhenResponseIsSuccessful()
    {
        // Arrange
        var tokenResponse = new TokenResponse { AccessToken = "fake-token" };
        var responseBody = JsonSerializer.Serialize(tokenResponse);
        _fixture.Handler.SetupResponse(HttpStatusCode.OK, responseBody);

        // Act
        var result = await _sut.GetAdminAccessTokenAsync();

        // Assert
        result.Should().Be("fake-token");
        _fixture.Handler.LastRequest!.RequestUri!.ToString().Should().Contain("/protocol/openid-connect/token");
    }

    [Fact]
    public async Task GetAdminAccessTokenAsync_ShouldThrow_WhenResponseIsFailure()
    {
        // Arrange
        _fixture.Handler.SetupResponse(HttpStatusCode.BadRequest, "bad request");

        // Act
        Func<Task> act = () => _sut.GetAdminAccessTokenAsync();

        // Assert
        await act.Should().ThrowAsync<InvalidOperationException>().WithMessage("*Failed to obtain admin access token from OAuth*");
    }
}
