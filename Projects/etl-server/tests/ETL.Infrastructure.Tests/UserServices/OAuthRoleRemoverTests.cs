using System.Text.Json;
using ETL.Application.Common.Options;
using ETL.Infrastructure.OAuthClients.Abstractions;
using ETL.Infrastructure.UserServices;
using FluentAssertions;
using Microsoft.Extensions.Options;
using NSubstitute;

namespace ETL.Infrastructure.Tests.UserServices;

public class OAuthRoleRemoverTests
{
    private readonly IOAuthGetJsonArray _getArray;
    private readonly IOAuthDeleteJson _delete;
    private readonly IOptions<AuthOptions> _options;
    private readonly OAuthRoleRemover _sut;

    public OAuthRoleRemoverTests()
    {
        _getArray = Substitute.For<IOAuthGetJsonArray>();
        _delete = Substitute.For<IOAuthDeleteJson>();
        _options = Options.Create(new AuthOptions { Realm = "realm" });
        _sut = new OAuthRoleRemover(_getArray, _delete, _options);
    }

    [Fact]
    public void Constructor_ShouldThrow_When_GetArrayIsNull()
    {
        // Act
        Action act = () => new OAuthRoleRemover(null!, _delete, _options);

        // Assert
        act.Should().Throw<ArgumentNullException>().WithParameterName("getArray");
    }

    [Fact]
    public void Constructor_ShouldThrow_When_DeleteIsNull()
    {
        // Act
        Action act = () => new OAuthRoleRemover(_getArray, null!, _options);

        // Assert
        act.Should().Throw<ArgumentNullException>().WithParameterName("delete");
    }

    [Fact]
    public void Constructor_ShouldThrow_When_OptionsIsNull()
    {
        // Act
        Action act = () => new OAuthRoleRemover(_getArray, _delete, null!);

        // Assert
        act.Should().Throw<ArgumentNullException>().WithParameterName("options");
    }

    [Fact]
    public async Task RemoveAllRealmRolesAsync_ShouldNotCallDelete_When_NoRolesReturned()
    {
        // Arrange
        var userId = "u1";
        _getArray.GetJsonArrayAsync(Arg.Any<string>(), Arg.Any<CancellationToken>())
            .Returns(Task.FromResult(new List<JsonElement>()));

        // Act
        await _sut.RemoveAllRealmRolesAsync(userId, CancellationToken.None);

        // Assert
        await _delete.DidNotReceiveWithAnyArgs().DeleteJsonAsync(Arg.Any<string>(), Arg.Any<object?>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task RemoveAllRealmRolesAsync_ShouldCallDelete_When_RolesExist()
    {
        // Arrange
        var userId = "u1";
        var roleJson = JsonDocument.Parse("{\"id\":\"r1\"}").RootElement;
        var roles = new List<JsonElement> { roleJson };

        _getArray.GetJsonArrayAsync(Arg.Any<string>(), Arg.Any<CancellationToken>())
            .Returns(Task.FromResult(roles));

        // Act
        await _sut.RemoveAllRealmRolesAsync(userId, CancellationToken.None);

        // Assert
        await _delete.Received(1).DeleteJsonAsync(
                Arg.Is<string>(s => s.Contains($"/users/{Uri.EscapeDataString(userId)}/role-mappings/realm")),
                Arg.Is<List<JsonElement>>(l => l != null && l.Count == 1),
                Arg.Any<CancellationToken>());

    }

    [Fact]
    public async Task RemoveAllRealmRolesAsync_ShouldTreatNullAsEmptyList()
    {
        // Arrange
        var userId = "u1";
        _getArray.GetJsonArrayAsync(Arg.Any<string>(), Arg.Any<CancellationToken>())
            .Returns(Task.FromResult<List<JsonElement>>(null!));

        // Act
        var exception = await Record.ExceptionAsync(() => _sut.RemoveAllRealmRolesAsync(userId, CancellationToken.None));

        // Assert
        exception.Should().BeNull();
        await _delete.DidNotReceiveWithAnyArgs().DeleteJsonAsync(Arg.Any<string>(), Arg.Any<object?>(), Arg.Any<CancellationToken>());
    }
}
