using System.Text.Json;
using ETL.Application.Common.DTOs;
using ETL.Infrastructure.UserServices;
using ETL.Infrastructure.UserServices.Abstractions;
using FluentAssertions;
using NSubstitute;

namespace ETL.Infrastructure.Tests.UserServices;

public class OAuthAllUserReaderTests
{
    private readonly IUserFetcher _userFetcher;
    private readonly IUserJsonMapper _userMapper;
    private readonly IUserRoleAssigner _roleAssigner;
    private readonly OAuthAllUserReader _sut;

    public OAuthAllUserReaderTests()
    {
        _userFetcher = Substitute.For<IUserFetcher>();
        _userMapper = Substitute.For<IUserJsonMapper>();
        _roleAssigner = Substitute.For<IUserRoleAssigner>();

        _sut = new OAuthAllUserReader(_userFetcher, _userMapper, _roleAssigner);
    }

    [Fact]
    public void Constructor_ShouldThrow_WhenUserFetcherIsNull()
    {
        // Act
        Action act = () => new OAuthAllUserReader(null!, _userMapper, _roleAssigner);

        // Assert
        act.Should().Throw<ArgumentNullException>().WithParameterName("userFetcher");
    }

    [Fact]
    public void Constructor_ShouldThrow_WhenUserMapperIsNull()
    {
        // Act
        Action act = () => new OAuthAllUserReader(_userFetcher, null!, _roleAssigner);

        // Assert
        act.Should().Throw<ArgumentNullException>().WithParameterName("userMapper");
    }

    [Fact]
    public void Constructor_ShouldThrow_WhenRoleAssignerIsNull()
    {
        // Act
        Action act = () => new OAuthAllUserReader(_userFetcher, _userMapper, null!);

        // Assert
        act.Should().Throw<ArgumentNullException>().WithParameterName("roleAssigner");
    }

    [Fact]
    public async Task GetAllAsync_ShouldReturnMappedUsersAndAssignedRoles_WhenSuccessful()
    {
        // Arrange
        var raw = new List<JsonElement>
        {
            JsonDocument.Parse("{\"id\":\"1\",\"username\":\"u1\"}").RootElement,
            JsonDocument.Parse("{\"id\":\"2\",\"username\":\"u2\"}").RootElement
        };

        var mapped = new List<UserDto>
        {
            new UserDto { Id = "1", Username = "u1", Email = "", FirstName = "", LastName = "", Role = null },
            new UserDto { Id = "2", Username = "u2", Email = "", FirstName = "", LastName = "", Role = null }
        };

        _userFetcher.FetchAllUsersRawAsync(Arg.Any<int?>(), Arg.Any<int?>(), Arg.Any<CancellationToken>())
            .Returns(Task.FromResult(raw));

        _userMapper.Map(raw[0]).Returns(mapped[0]);
        _userMapper.Map(raw[1]).Returns(mapped[1]);

        _roleAssigner
            .When(r => r.AssignRolesAsync(Arg.Any<List<UserDto>>(), Arg.Any<IEnumerable<string>>(), Arg.Any<CancellationToken>()))
            .Do(ci =>
            {
                var usersArg = ci.ArgAt<List<UserDto>>(0);
                usersArg[0].Role = "Admin";
                usersArg[1].Role = "User";
            });

        var expected = new List<UserDto>
        {
            new UserDto { Id = "1", Username = "u1", Email = "", FirstName = "", LastName = "", Role = "Admin" },
            new UserDto { Id = "2", Username = "u2", Email = "", FirstName = "", LastName = "", Role = "User" }
        };

        // Act
        var result = await _sut.GetAllAsync();

        // Assert
        result.Should().BeEquivalentTo(expected);
    }

    [Fact]
    public async Task GetAllAsync_ShouldReturnEmptyList_WhenFetcherReturnsEmpty()
    {
        // Arrange
        _userFetcher.FetchAllUsersRawAsync(Arg.Any<int?>(), Arg.Any<int?>(), Arg.Any<CancellationToken>())
            .Returns(Task.FromResult(new List<JsonElement>()));

        var expected = new List<UserDto>();

        // Act
        var result = await _sut.GetAllAsync();

        // Assert
        result.Should().BeEquivalentTo(expected);
    }
}
