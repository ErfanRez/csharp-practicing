using System.Text.Json;
using ETL.Application.Common.Options;
using ETL.Infrastructure.OAuthClients.Abstractions;
using ETL.Infrastructure.UserServices;
using FluentAssertions;
using Microsoft.Extensions.Options;
using NSubstitute;

namespace ETL.Infrastructure.Tests.UserServices;

public class UsersRoleFetcherTests
{
    private readonly IOAuthGetJsonArray _getArray;
    private readonly UsersRoleFetcher _sut;

    public UsersRoleFetcherTests()
    {
        _getArray = Substitute.For<IOAuthGetJsonArray>();
        var authOptions = new AuthOptions { Realm = "realmX" };
        _sut = new UsersRoleFetcher(_getArray, Options.Create(authOptions));
    }

    [Fact]
    public void Constructor_ShouldThrowArgumentNullException_WhenGetArrayIsNull()
    {
        // Arrange // Act
        Action act = () => new UsersRoleFetcher(null!, Options.Create(new AuthOptions()));

        // Assert
        act.Should().Throw<ArgumentNullException>().WithParameterName("getArray");
    }

    [Fact]
    public void Constructor_ShouldThrowArgumentNullException_WhenOptionsIsNull()
    {
        // Arrange // Act
        Action act = () => new UsersRoleFetcher(Substitute.For<IOAuthGetJsonArray>(), null!);

        // Assert
        act.Should().Throw<ArgumentNullException>().WithParameterName("options");
    }

    [Fact]
    public async Task FetchUsersForRoleAsync_ShouldReturnEmpty_WhenRoleNameIsNullOrWhitespace()
    {
        // Arrange // Act
        var result1 = await _sut.FetchUsersForRoleAsync(string.Empty);
        var result2 = await _sut.FetchUsersForRoleAsync("   ");

        // Assert
        result1.Should().BeEmpty();
        result2.Should().BeEmpty();
    }

    [Fact]
    public async Task FetchUsersForRoleAsync_ShouldCallGetArrayWithEncodedRole_WhenRoleProvided()
    {
        // Arrange
        var roleName = "Admin Role";
        var list = new List<JsonElement> { JsonDocument.Parse("{\"id\":\"u1\"}").RootElement };
        _getArray.GetJsonArrayAsync(Arg.Any<string>(), Arg.Any<CancellationToken>()).Returns(Task.FromResult(list));

        // Act
        var result = await _sut.FetchUsersForRoleAsync(roleName, CancellationToken.None);

        // Assert
        result.Should().HaveCount(1);
        await _getArray.Received(1).GetJsonArrayAsync(
            Arg.Is<string>(s => s.Contains("/roles/") && s.Contains(Uri.EscapeDataString(roleName))),
            Arg.Any<CancellationToken>());
    }
}
