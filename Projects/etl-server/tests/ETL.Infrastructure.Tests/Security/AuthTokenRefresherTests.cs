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
public class AuthTokenRefresherTests
{
    private readonly HttpClientTestFixture _fixture;
    private readonly AuthTokenRefresher _sut;

    public AuthTokenRefresherTests(HttpClientTestFixture fixture)
    {
        _fixture = fixture;

        var authOptions = new AuthOptions
        {
            Authority = "https://fake-auth",
            ClientId = "client-id",
            ClientSecret = "client-secret"
        };
        var options = Options.Create(authOptions);

        var httpFactory = Substitute.For<IHttpClientFactory>();
        httpFactory.CreateClient().Returns(_fixture.Client);

        _sut = new AuthTokenRefresher(httpFactory, options);
    }

    [Fact]
    public void Constructor_ShouldThrowArgumentNullException_WhenHttpClientFactoryIsNull()
    {
        // Arrange // Act
        Action act = () => new AuthTokenRefresher(null!, Options.Create(new AuthOptions()));

        // Assert
        act.Should().Throw<ArgumentNullException>().WithParameterName("httpClientFactory");
    }

    [Fact]
    public void Constructor_ShouldThrowArgumentNullException_WhenOptionsIsNull()
    {
        // Arrange // Act
        Action act = () => new AuthTokenRefresher(Substitute.For<IHttpClientFactory>(), null!);

        // Assert
        act.Should().Throw<ArgumentNullException>().WithParameterName("options");
    }

    [Fact]
    public async Task RefreshAsync_ShouldReturnTokens_WhenResponseIsSuccessful()
    {
        // Arrange
        var expectedResponse = new TokenResponse
        {
            AccessToken = "access",
            RefreshToken = "refresh",
            AccessExpiresIn = 3600
        };
        var responseBody = JsonSerializer.Serialize(expectedResponse);
        _fixture.Handler.SetupResponse(HttpStatusCode.OK, responseBody);

        // Act
        var result = await _sut.RefreshAsync("refresh-token", CancellationToken.None);

        // Assert
        result.AccessToken.Should().Be("access");
        _fixture.Handler.LastRequest!.RequestUri!.ToString().Should().Contain("/protocol/openid-connect/token");
    }

    [Fact]
    public async Task RefreshAsync_ShouldThrowInvalidOperationException_WhenResponseIsFailure()
    {
        // Arrange
        _fixture.Handler.SetupResponse(HttpStatusCode.BadRequest, "bad request");

        // Act // Assert
        await FluentActions.Awaiting(() => _sut.RefreshAsync("refresh-token", CancellationToken.None))
            .Should()
            .ThrowAsync<InvalidOperationException>()
            .WithMessage("*bad request*");
    }

    [Fact]
    public async Task RefreshAsync_ShouldThrowInvalidOperationException_WhenTokensCannotBeParsed()
    {
        // Arrange
        _fixture.Handler.SetupResponse(HttpStatusCode.OK, "{}");

        // Act // Assert
        await FluentActions.Awaiting(() => _sut.RefreshAsync("refresh-token", CancellationToken.None))
            .Should()
            .ThrowAsync<InvalidOperationException>()
            .WithMessage("*Failed to parse tokens*");
    }
}
