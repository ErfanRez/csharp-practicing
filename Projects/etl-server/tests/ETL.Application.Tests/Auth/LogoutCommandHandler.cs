using ETL.Application.Abstractions.Security;
using ETL.Application.Auth;
using FluentAssertions;
using NSubstitute;

namespace ETL.Application.Tests.Auth;

public class LogoutCommandHandlerTests
{
    private readonly IAuthLogoutService _logoutService;
    private readonly LogoutCommandHandler _sut;

    public LogoutCommandHandlerTests()
    {
        _logoutService = Substitute.For<IAuthLogoutService>();
        _sut = new LogoutCommandHandler(_logoutService);
    }

    [Fact]
    public void Constructor_ShouldThrowArgumentNullException_WhenLogoutServiceIsNull()
    {
        // Arrange // Act
        Action act = () => new LogoutCommandHandler(null!);

        // Assert
        act.Should().Throw<ArgumentNullException>().WithParameterName("logoutService");
    }

    [Fact]
    public async Task Handle_ShouldReturnSuccess_WhenLogoutSucceeds()
    {
        // Arrange
        var command = new LogoutCommand("access", "refresh");
        _logoutService
            .LogoutAsync("access", "refresh", Arg.Any<CancellationToken>())
            .Returns(Task.CompletedTask);

        // Act
        var result = await _sut.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        await _logoutService.Received(1).LogoutAsync("access", "refresh", Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_ShouldReturnFailure_WhenLogoutThrows()
    {
        // Arrange
        var command = new LogoutCommand("access", "refresh");
        _logoutService
            .LogoutAsync("access", "refresh", Arg.Any<CancellationToken>())
            .Returns<Task>(x => throw new InvalidOperationException("Service unavailable"));

        // Act
        var result = await _sut.Handle(command, CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Code.Should().Be("Auth.LogOut.Failed");
    }
}
