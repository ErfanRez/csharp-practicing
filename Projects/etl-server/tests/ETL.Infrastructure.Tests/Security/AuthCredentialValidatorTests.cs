using System.Net;
using ETL.Application.Common.Options;
using ETL.Infrastructure.Security;
using ETL.Infrastructure.Tests.HttpClientFixture;
using FluentAssertions;
using Microsoft.Extensions.Options;
using NSubstitute;

namespace ETL.Infrastructure.Tests.Security;

[Collection("HttpClient collection")]
public class AuthCredentialValidatorTests
{
    private readonly HttpClientTestFixture _fixture;
    private readonly AuthCredentialValidator _sut;

    public AuthCredentialValidatorTests(HttpClientTestFixture fixture)
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

        _sut = new AuthCredentialValidator(httpFactory, options);
    }

    [Fact]
    public void Constructor_ShouldThrowArgumentNullException_WhenHttpClientFactoryIsNull()
    {
        // Arrange // Act
        Action act = () => new AuthCredentialValidator(null!, Options.Create(new AuthOptions()));

        // Assert
        act.Should().Throw<ArgumentNullException>().WithParameterName("httpClientFactory");
    }

    [Fact]
    public void Constructor_ShouldThrowArgumentNullException_WhenOptionsIsNull()
    {
        // Arrange // Act
        Action act = () => new AuthCredentialValidator(Substitute.For<IHttpClientFactory>(), null!);

        // Assert
        act.Should().Throw<ArgumentNullException>().WithParameterName("options");
    }

    [Fact]
    public async Task ValidateCredentialsAsync_ShouldReturnTrue_WhenResponseIsSuccessful()
    {
        // Arrange
        _fixture.Handler.SetupResponse(HttpStatusCode.OK, "{}");

        // Act
        var result = await _sut.ValidateCredentialsAsync("user", "pass");

        // Assert
        result.Should().BeTrue();
        _fixture.Handler.LastRequest!.RequestUri!.ToString().Should().Contain("/protocol/openid-connect/token");
    }

    [Fact]
    public async Task ValidateCredentialsAsync_ShouldReturnFalse_WhenResponseIsFailure()
    {
        // Arrange
        _fixture.Handler.SetupResponse(HttpStatusCode.Unauthorized, "invalid creds");

        // Act
        var result = await _sut.ValidateCredentialsAsync("user", "wrongpass");

        // Assert
        result.Should().BeFalse();
    }
}
