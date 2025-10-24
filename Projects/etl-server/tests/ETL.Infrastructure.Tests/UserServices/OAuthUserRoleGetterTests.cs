using System.Text.Json;
using ETL.Application.Common.Options;
using ETL.Infrastructure.OAuthClients.Abstractions;
using ETL.Infrastructure.UserServices;
using FluentAssertions;
using Microsoft.Extensions.Options;
using NSubstitute;

namespace ETL.Infrastructure.Tests.UserServices;

public class OAuthUserRoleGetterTests
{
    private readonly IOAuthGetJsonArray _getArray;
    private readonly IOptions<AuthOptions> _options;
    private readonly OAuthUserRoleGetter _sut;

    public OAuthUserRoleGetterTests()
    {
        _getArray = Substitute.For<IOAuthGetJsonArray>();
        _options = Options.Create(new AuthOptions { Realm = "myrealm" });
        _sut = new OAuthUserRoleGetter(_getArray, _options);
    }

    [Fact]
    public void Constructor_ShouldThrowArgumentNullException_WhenGetArrayIsNull()
    {
        // Act
        Action act = () => new OAuthUserRoleGetter(null!, _options);

        // Assert
        act.Should().Throw<ArgumentNullException>().WithParameterName("getArray");
    }

    [Fact]
    public void Constructor_ShouldThrowArgumentNullException_WhenOptionsIsNull()
    {
        // Act
        Action act = () => new OAuthUserRoleGetter(_getArray, null!);

        // Assert
        act.Should().Throw<ArgumentNullException>().WithParameterName("options");
    }

    [Fact]
    public async Task GetRoleForUserAsync_ShouldReturnNull_When_ArrayIsNullOrEmpty()
    {
        // Arrange
        var userId = "u1";
        _getArray.GetJsonArrayAsync(Arg.Any<string>(), Arg.Any<CancellationToken>())
            .Returns(Task.FromResult<List<JsonElement>>(null!));

        // Act
        var result = await _sut.GetRoleForUserAsync(userId);

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public async Task GetRoleForUserAsync_ShouldReturnFirstRoleName_WhenArrayContainsRole()
    {
        // Arrange
        var userId = "u1";
        var element = JsonDocument.Parse("{\"name\":\"admin\"}").RootElement;
        _getArray.GetJsonArrayAsync(Arg.Any<string>(), Arg.Any<CancellationToken>())
            .Returns(Task.FromResult(new List<JsonElement> { element }));

        // Act
        var result = await _sut.GetRoleForUserAsync(userId);

        // Assert
        result.Should().Be("admin");
    }

    [Fact]
    public async Task GetRoleForUserAsync_ShouldSkipElementsWithoutNameProperty()
    {
        // Arrange
        var userId = "u1";
        var role1 = JsonDocument.Parse("{\"id\":\"123\"}").RootElement;
        var role2 = JsonDocument.Parse("{\"name\":\"editor\"}").RootElement;
        _getArray.GetJsonArrayAsync(Arg.Any<string>(), Arg.Any<CancellationToken>())
            .Returns(Task.FromResult(new List<JsonElement> { role1, role2 }));

        // Act
        var result = await _sut.GetRoleForUserAsync(userId);

        // Assert
        result.Should().Be("editor");
    }
}
