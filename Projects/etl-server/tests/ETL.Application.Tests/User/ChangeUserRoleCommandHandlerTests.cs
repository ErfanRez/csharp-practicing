using System.Net;
using ETL.Application.Abstractions.UserServices;
using ETL.Application.User.ChangeRole;
using FluentAssertions;
using NSubstitute;

namespace ETL.Application.Tests.User;

public class ChangeUserRoleCommandHandlerTests
{
    private readonly IOAuthRoleRemover _roleRemover;
    private readonly IOAuthRoleAssigner _roleAssigner;
    private readonly ChangeUserRoleCommandHandler _sut;

    public ChangeUserRoleCommandHandlerTests()
    {
        _roleRemover = Substitute.For<IOAuthRoleRemover>();
        _roleAssigner = Substitute.For<IOAuthRoleAssigner>();
        _sut = new ChangeUserRoleCommandHandler(_roleRemover, _roleAssigner);
    }

    [Fact]
    public void Constructor_ShouldThrow_When_RoleRemoverIsNull()
    {
        // Act
        Action act = () => new ChangeUserRoleCommandHandler(null!, _roleAssigner);

        // Assert
        act.Should().Throw<ArgumentNullException>().WithParameterName("roleRemover");
    }

    [Fact]
    public void Constructor_ShouldThrow_When_RoleAssignerIsNull()
    {
        // Act
        Action act = () => new ChangeUserRoleCommandHandler(_roleRemover, null!);

        // Assert
        act.Should().Throw<ArgumentNullException>().WithParameterName("roleAssigner");
    }

    [Fact]
    public async Task Handle_ShouldReturnSuccess_When_RoleChangeSucceeds()
    {
        // Arrange
        var command = new ChangeUserRoleCommand("user123", "Admin");
        _roleRemover.RemoveAllRealmRolesAsync(command.UserId, Arg.Any<CancellationToken>())
            .Returns(Task.CompletedTask);
        _roleAssigner.AssignRoleAsync(command.UserId, command.Role, Arg.Any<CancellationToken>())
            .Returns(Task.CompletedTask);

        // Act
        var result = await _sut.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
    }

    [Fact]
    public async Task Handle_ShouldReturnNotFoundFailure_When_RemoverThrowsNotFoundHttpRequestException()
    {
        // Arrange
        var command = new ChangeUserRoleCommand("user123", "Admin");
        _roleRemover.RemoveAllRealmRolesAsync(command.UserId, Arg.Any<CancellationToken>())
            .Returns<Task>(_ => throw new HttpRequestException("not found", null, HttpStatusCode.NotFound));

        // Act
        var result = await _sut.Handle(command, CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Code.Should().Be("OAuth.NotFound");
    }

    [Fact]
    public async Task Handle_ShouldReturnProblemFailure_When_AssignerThrowsException()
    {
        // Arrange
        var command = new ChangeUserRoleCommand("user123", "Admin");
        _roleRemover.RemoveAllRealmRolesAsync(command.UserId, Arg.Any<CancellationToken>())
            .Returns(Task.CompletedTask);
        _roleAssigner.AssignRoleAsync(command.UserId, command.Role, Arg.Any<CancellationToken>())
            .Returns<Task>(_ => throw new Exception("boom"));

        // Act
        var result = await _sut.Handle(command, CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Code.Should().Be("User.ChangeRole.Exception");
    }
}
