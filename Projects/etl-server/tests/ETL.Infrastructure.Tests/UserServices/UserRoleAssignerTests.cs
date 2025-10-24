using System.Text.Json;
using ETL.Application.Common.DTOs;
using ETL.Infrastructure.UserServices;
using ETL.Infrastructure.UserServices.Abstractions;
using FluentAssertions;
using NSubstitute;

namespace ETL.Infrastructure.Tests.UserServices;

public class UserRoleAssignerTests
{
    private readonly IUsersRoleFetcher _roleUsersFetcher;
    private readonly UserRoleAssigner _sut;

    public UserRoleAssignerTests()
    {
        _roleUsersFetcher = Substitute.For<IUsersRoleFetcher>();
        _sut = new UserRoleAssigner(_roleUsersFetcher);
    }

    [Fact]
    public void Constructor_ShouldThrow_WhenRoleUsersFetcherIsNull()
    {
        // Act
        Action act = () => new UserRoleAssigner(null!);

        // Assert
        act.Should().Throw<ArgumentNullException>().WithParameterName("roleUsersFetcher");
    }

    [Fact]
    public async Task AssignRolesAsync_ShouldNotCallFetcher_WhenUsersIsNullOrEmpty()
    {
        // Arrange

        // Act
        await _sut.AssignRolesAsync(null!, new List<string>(), CancellationToken.None);

        // Assert
        await _roleUsersFetcher.DidNotReceiveWithAnyArgs().FetchUsersForRoleAsync(Arg.Any<string>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task AssignRolesAsync_ShouldAssignRoles_WhenRoleUsersFetched()
    {
        // Arrange
        var users = new List<UserDto>
        {
            new UserDto { Id = "u1", Username = "u1", Email = "", FirstName = "", LastName = "" },
            new UserDto { Id = "u2", Username = "u2", Email = "", FirstName = "", LastName = "" }
        };

        var rolesToCheck = new List<string> { "Admin", "User" };

        var adminList = new List<JsonElement>
        {
            JsonDocument.Parse("{\"id\":\"u1\"}").RootElement
        };
        var userList = new List<JsonElement>
        {
            JsonDocument.Parse("{\"id\":\"u2\"}").RootElement
        };

        _roleUsersFetcher.FetchUsersForRoleAsync("Admin", Arg.Any<CancellationToken>()).Returns(Task.FromResult(adminList));
        _roleUsersFetcher.FetchUsersForRoleAsync("User", Arg.Any<CancellationToken>()).Returns(Task.FromResult(userList));

        var expected = new List<UserDto>
        {
            new UserDto { Id = "u1", Username = "u1", Email = "", FirstName = "", LastName = "", Role = "Admin" },
            new UserDto { Id = "u2", Username = "u2", Email = "", FirstName = "", LastName = "", Role = "User" }
        };

        // Act
        await _sut.AssignRolesAsync(users, rolesToCheck, CancellationToken.None);

        // Assert
        users.Should().BeEquivalentTo(expected);
    }
}
