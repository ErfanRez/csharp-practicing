using ETL.Application.Abstractions.Security;
using ETL.Application.Auth;
using ETL.Application.Common.DTOs;
using FluentAssertions;
using NSubstitute;

namespace ETL.Application.Tests.Auth;

public class CallbackCommandHandlerTests
{
    private readonly IAuthCodeForTokenExchanger _exchanger;
    private readonly CallbackCommandHandler _sut;

    public CallbackCommandHandlerTests()
    {
        _exchanger = Substitute.For<IAuthCodeForTokenExchanger>();
        _sut = new CallbackCommandHandler(_exchanger);
    }

    [Fact]
    public void Constructor_ShouldThrowArgumentNullException_WhenExchangerIsNull()
    {
        // Arrange // Act
        Action act = () => new CallbackCommandHandler(null!);

        // Assert
        act.Should().Throw<ArgumentNullException>().WithParameterName("exchanger");
    }

    [Fact]
    public async Task Handle_ShouldReturnFailure_WhenExchangerThrows()
    {
        // Arrange
        var command = new LoginCallbackCommand("authcode", "redirect");
        _exchanger
            .ExchangeCodeForTokensAsync("authcode", "redirect", Arg.Any<CancellationToken>())
            .Returns<Task<TokenResponse>>(x => throw new InvalidOperationException("oops"));

        // Act
        var result = await _sut.Handle(command, CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Code.Should().Be("Token.Exchange.Failed");
    }

    [Fact]
    public async Task Handle_ShouldReturnSuccess_WhenExchangerReturnsSuccess_EvenIfAccessTokenIsEmpty()
    {
        // Arrange
        var command = new LoginCallbackCommand("authcode", "redirect");
        var expected = new TokenResponse
        {
            AccessToken = string.Empty,
            RefreshToken = "ref",
            AccessExpiresIn = 3600
        };

        _exchanger
            .ExchangeCodeForTokensAsync("authcode", "redirect", Arg.Any<CancellationToken>())
            .Returns(Task.FromResult(expected));

        // Act
        var result = await _sut.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().BeEquivalentTo(expected);
    }

    [Fact]
    public async Task Handle_ShouldReturnSuccess_WhenTokensAreValid()
    {
        // Arrange
        var command = new LoginCallbackCommand("authcode", "redirect");
        var expected = new TokenResponse
        {
            AccessToken = "valid_token",
            RefreshToken = "ref",
            AccessExpiresIn = 3600
        };

        _exchanger
            .ExchangeCodeForTokensAsync("authcode", "redirect", Arg.Any<CancellationToken>())
            .Returns(Task.FromResult(expected));

        // Act
        var result = await _sut.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().BeEquivalentTo(expected);
    }

    [Fact]
    public async Task Handle_ShouldPassEmptyRedirectPath_WhenNullProvided()
    {
        // Arrange
        var command = new LoginCallbackCommand("authcode", null);
        var expected = new TokenResponse { AccessToken = "valid" };

        _exchanger
            .ExchangeCodeForTokensAsync("authcode", "", Arg.Any<CancellationToken>())
            .Returns(Task.FromResult(expected));

        // Act
        var result = await _sut.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        await _exchanger.Received(1)
            .ExchangeCodeForTokensAsync("authcode", "", Arg.Any<CancellationToken>());
    }
}
