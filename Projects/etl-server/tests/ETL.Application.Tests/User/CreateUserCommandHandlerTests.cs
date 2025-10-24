using System.Net;
using ETL.Application.Abstractions.UserServices;
using ETL.Application.User.Create;
using FluentAssertions;
using NSubstitute;

namespace ETL.Application.Tests.User;

public class CreateUserCommandHandlerTests
{
    private readonly IOAuthUserCreator _userCreator;
    private readonly IOAuthRoleAssigner _roleAssigner;
    private readonly CreateUserCommandHandler _sut;

    public CreateUserCommandHandlerTests()
    {
        _userCreator = Substitute.For<IOAuthUserCreator>();
        _roleAssigner = Substitute.For<IOAuthRoleAssigner>();
        _sut = new CreateUserCommandHandler(_userCreator, _roleAssigner);
    }

    [Fact]
    public void Constructor_ShouldThrow_When_UserCreatorIsNull()
    {
        // Act
        Action act = () => new CreateUserCommandHandler(null!, _roleAssigner);

        // Assert
        act.Should().Throw<ArgumentNullException>().WithParameterName("userCreator");
    }

    [Fact]
    public void Constructor_ShouldThrow_When_RoleAssignerIsNull()
    {
        // Act
        Action act = () => new CreateUserCommandHandler(_userCreator, null!);

        // Assert
        act.Should().Throw<ArgumentNullException>().WithParameterName("roleAssigner");
    }

    [Fact]
    public async Task Handle_ShouldReturnSuccess_When_UserCreatedAndRoleAssigned()
    {
        // Arrange
        var command = new CreateUserCommand("testuser", null, null, null, "password", "admin");
        _userCreator.CreateUserAsync(command, Arg.Any<CancellationToken>())
            .Returns(Task.FromResult("new-user-id"));
        _roleAssigner.AssignRoleAsync("new-user-id", "admin", Arg.Any<CancellationToken>())
            .Returns(Task.CompletedTask);

        // Act
        var result = await _sut.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().Be("new-user-id");
    }

    [Fact]
    public async Task Handle_ShouldReturnConflictFailure_When_CreateUserThrowsConflictHttpRequestException()
    {
        // Arrange
        var command = new CreateUserCommand("testuser", null, null, null, "password", "admin");
        _userCreator.CreateUserAsync(command, Arg.Any<CancellationToken>())
            .Returns<Task<string>>(_ => throw new HttpRequestException("conflict", null, HttpStatusCode.Conflict));

        // Act
        var result = await _sut.Handle(command, CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Code.Should().Be("OAuth.User.Exists");
    }

    [Fact]
    public async Task Handle_ShouldReturnFailure_When_RoleAssignmentThrowsException()
    {
        // Arrange
        var command = new CreateUserCommand("testuser", null, null, null, "password", "admin");
        _userCreator.CreateUserAsync(command, Arg.Any<CancellationToken>())
            .Returns(Task.FromResult("new-user-id"));
        _roleAssigner.AssignRoleAsync("new-user-id", "admin", Arg.Any<CancellationToken>())
            .Returns<Task>(_ => throw new InvalidOperationException("boom"));

        // Act
        var result = await _sut.Handle(command, CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Code.Should().Be("User.Create.Failed");
    }

    [Fact]
    public async Task Handle_ShouldReturnFailure_When_CreateUserThrowsGeneralException()
    {
        // Arrange
        var command = new CreateUserCommand("testuser", null, null, null, "password", "");
        _userCreator.CreateUserAsync(command, Arg.Any<CancellationToken>())
            .Returns<Task<string>>(_ => throw new Exception("boom"));

        // Act
        var result = await _sut.Handle(command, CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Code.Should().Be("User.Create.Failed");
    }
}
