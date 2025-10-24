using System.Net;
using ETL.Application.Abstractions.UserServices;
using ETL.Application.User.Delete;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using NSubstitute;

namespace ETL.Application.Tests.User;

public class DeleteUserCommandHandlerTests
{
    private readonly IOAuthUserDeleter _userDeleter;
    private readonly ILogger<DeleteUserCommandHandler> _logger;
    private readonly DeleteUserCommandHandler _sut;

    public DeleteUserCommandHandlerTests()
    {
        _userDeleter = Substitute.For<IOAuthUserDeleter>();
        _logger = Substitute.For<ILogger<DeleteUserCommandHandler>>();
        _sut = new DeleteUserCommandHandler(_userDeleter, _logger);
    }

    [Fact]
    public async Task Handle_ShouldReturnSuccess_When_DeleteSucceeds()
    {
        // Arrange
        var command = new DeleteUserCommand("user123");
        _userDeleter.DeleteUserAsync(command.UserId, Arg.Any<CancellationToken>())
            .Returns(Task.CompletedTask);

        // Act
        var result = await _sut.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
    }

    [Fact]
    public async Task Handle_ShouldReturnNotFoundFailure_When_UserDeleterThrowsNotFound()
    {
        // Arrange
        var command = new DeleteUserCommand("user123");
        _userDeleter.DeleteUserAsync(command.UserId, Arg.Any<CancellationToken>())
            .Returns<Task>(_ => throw new HttpRequestException("not found", null, HttpStatusCode.NotFound));

        // Act
        var result = await _sut.Handle(command, CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Code.Should().Be("OAuth.UserNotFound");
    }

    [Fact]
    public async Task Handle_ShouldReturnProblemFailure_When_UserDeleterThrowsException()
    {
        // Arrange
        var command = new DeleteUserCommand("user123");
        _userDeleter.DeleteUserAsync(command.UserId, Arg.Any<CancellationToken>())
            .Returns<Task>(_ => throw new Exception("boom"));

        // Act
        var result = await _sut.Handle(command, CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Code.Should().Be("User.Delete.Failed");
    }

    [Fact]
    public void Constructor_ShouldThrow_When_UserDeleterIsNull()
    {
        // Act
        Action act = () => new DeleteUserCommandHandler(null!, _logger);

        // Assert
        act.Should().Throw<ArgumentNullException>().WithParameterName("userDeleter");
    }

    [Fact]
    public void Constructor_ShouldNotThrow_When_LoggerIsNull()
    {
        // Act
        Action act = () => new DeleteUserCommandHandler(_userDeleter, null!);

        // Assert
        act.Should().NotThrow();
    }
}
