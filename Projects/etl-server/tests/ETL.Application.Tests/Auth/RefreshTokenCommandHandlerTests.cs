using ETL.Application.Abstractions.Security;
using ETL.Application.Auth;
using ETL.Application.Common.DTOs;
using FluentAssertions;
using NSubstitute;

namespace ETL.Application.Tests.Auth;

public class RefreshTokenCommandHandlerTests
{
    private readonly IAuthTokenRefresher _tokenRefresher;
    private readonly RefreshTokenCommandHandler _sut;

    public RefreshTokenCommandHandlerTests()
    {
        _tokenRefresher = Substitute.For<IAuthTokenRefresher>();
        _sut = new RefreshTokenCommandHandler(_tokenRefresher);
    }

    [Fact]
    public void Constructor_ShouldThrowArgumentNullException_WhenTokenRefresherIsNull()
    {
        // Arrange // Act
        Action act = () => new RefreshTokenCommandHandler(null!);

        // Assert
        act.Should().Throw<ArgumentNullException>().WithParameterName("tokenRefresher");
    }

    [Fact]
    public async Task Handle_ShouldReturnFailure_WhenRefreshTokenIsMissing()
    {
        // Arrange
        var command = new RefreshTokenCommand(string.Empty);

        // Act
        var result = await _sut.Handle(command, CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Code.Should().Be("Auth.Refresh.MissingToken");
    }

    [Fact]
    public async Task Handle_ShouldReturnSuccess_WhenRefresherReturnsTokens()
    {
        // Arrange
        var command = new RefreshTokenCommand("refresh-token");
        var expected = new TokenResponse { AccessToken = "access", RefreshToken = "r", AccessExpiresIn = 3600 };

        _tokenRefresher
            .RefreshAsync("refresh-token", Arg.Any<CancellationToken>())
            .Returns(Task.FromResult(expected));

        // Act
        var result = await _sut.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().BeEquivalentTo(expected);
    }

    [Fact]
    public async Task Handle_ShouldReturnFailure_WhenRefresherThrows()
    {
        // Arrange
        var command = new RefreshTokenCommand("refresh-token");
        _tokenRefresher
            .RefreshAsync("refresh-token", Arg.Any<CancellationToken>())
            .Returns<Task<TokenResponse>>(x => throw new InvalidOperationException("boom"));

        // Act
        var result = await _sut.Handle(command, CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Code.Should().Be("Auth.Refresh.Failed");
    }
}
