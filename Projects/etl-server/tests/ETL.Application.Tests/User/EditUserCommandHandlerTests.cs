using System.Net;
using ETL.Application.Abstractions.UserServices;
using ETL.Application.User.Edit;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using NSubstitute;

namespace ETL.Application.Tests.User;

public class EditUserCommandHandlerTests
{
    private readonly IOAuthUserUpdater _userUpdater;
    private readonly ILogger<EditUserCommandHandler> _logger;
    private readonly EditUserCommandHandler _sut;

    public EditUserCommandHandlerTests()
    {
        _userUpdater = Substitute.For<IOAuthUserUpdater>();
        _logger = Substitute.For<ILogger<EditUserCommandHandler>>();
        _sut = new EditUserCommandHandler(_userUpdater, _logger);
    }

    [Fact]
    public async Task Handle_ShouldReturnSuccess_When_UpdateSucceeds()
    {
        // Arrange
        var command = new EditUserCommand("u1", "alice", "a@b.com", "A", "B");
        _userUpdater.UpdateUserAsync(command, Arg.Any<CancellationToken>()).Returns(Task.CompletedTask);

        // Act
        var result = await _sut.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
    }

    [Fact]
    public async Task Handle_ShouldReturnNotFoundFailure_When_UserUpdaterThrowsNotFound()
    {
        // Arrange
        var command = new EditUserCommand("u1", null, null, null, null);
        _userUpdater.UpdateUserAsync(command, Arg.Any<CancellationToken>())
            .Returns<Task>(_ => throw new HttpRequestException("not found", null, HttpStatusCode.NotFound));

        // Act
        var result = await _sut.Handle(command, CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Code.Should().Be("OAuth.UserNotFound");
    }

    [Fact]
    public async Task Handle_ShouldReturnProblemFailure_When_UserUpdaterThrowsException()
    {
        // Arrange
        var command = new EditUserCommand("u1", null, null, null, null);
        _userUpdater.UpdateUserAsync(command, Arg.Any<CancellationToken>())
            .Returns<Task>(_ => throw new Exception("boom"));

        // Act
        var result = await _sut.Handle(command, CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Code.Should().Be("User.Edit.Failed");
    }

    [Fact]
    public void Constructor_ShouldThrow_When_UserUpdaterIsNull()
    {
        // Act
        Action act = () => new EditUserCommandHandler(null!, _logger);

        // Assert
        act.Should().Throw<ArgumentNullException>().WithParameterName("userUpdater");
    }

    [Fact]
    public void Constructor_ShouldNotThrow_When_LoggerIsNull()
    {
        // Act
        Action act = () => new EditUserCommandHandler(_userUpdater, null!);

        // Assert
        act.Should().NotThrow();
    }
}
