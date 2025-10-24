using System.Text.Json;
using ETL.Application.Common.Options;
using ETL.Infrastructure.OAuthClients.Abstractions;
using ETL.Infrastructure.UserServices;
using FluentAssertions;
using Microsoft.Extensions.Options;
using NSubstitute;

namespace ETL.Infrastructure.Tests.UserServices;

public class OAuthRoleAssignerTests
{
    private readonly IOAuthGetJson _getJson;
    private readonly IOAuthPostJson _postJson;
    private readonly IOptions<AuthOptions> _options;
    private readonly OAuthRoleAssigner _sut;

    public OAuthRoleAssignerTests()
    {
        _getJson = Substitute.For<IOAuthGetJson>();
        _postJson = Substitute.For<IOAuthPostJson>();
        _options = Options.Create(new AuthOptions { Realm = "myrealm" });
        _sut = new OAuthRoleAssigner(_getJson, _postJson, _options);
    }

    [Fact]
    public void Constructor_ShouldThrow_When_GetJsonIsNull()
    {
        // Act
        Action act = () => new OAuthRoleAssigner(null!, _postJson, _options);

        // Assert
        act.Should().Throw<ArgumentNullException>().WithParameterName("getJson");
    }

    [Fact]
    public void Constructor_ShouldThrow_When_PostJsonIsNull()
    {
        // Act
        Action act = () => new OAuthRoleAssigner(_getJson, null!, _options);

        // Assert
        act.Should().Throw<ArgumentNullException>().WithParameterName("postJson");
    }

    [Fact]
    public void Constructor_ShouldThrow_When_OptionsIsNull()
    {
        // Act
        Action act = () => new OAuthRoleAssigner(_getJson, _postJson, null!);

        // Assert
        act.Should().Throw<ArgumentNullException>().WithParameterName("options");
    }

    [Fact]
    public async Task AssignRoleAsync_ShouldNotCall_When_RoleNameIsEmpty()
    {
        // Arrange
        var userId = "u1";
        var roleName = "   ";

        // Act
        await _sut.AssignRoleAsync(userId, roleName, CancellationToken.None);

        // Assert
        await _getJson.DidNotReceiveWithAnyArgs().GetJsonAsync(Arg.Any<string>(), Arg.Any<CancellationToken>());
        await _postJson.DidNotReceiveWithAnyArgs().PostJsonAsync(Arg.Any<string>(), Arg.Any<object>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task AssignRoleAsync_ShouldCallGetAndPost_When_RoleProvided()
    {
        // Arrange
        var userId = "u1";
        var roleName = "admin";
        var roleDef = JsonDocument.Parse("{\"id\":\"role-id\",\"name\":\"admin\"}").RootElement;

        _getJson.GetJsonAsync(Arg.Any<string>(), Arg.Any<CancellationToken>())
            .Returns(Task.FromResult(roleDef));
        _postJson.PostJsonAsync(Arg.Any<string>(), Arg.Any<object>(), Arg.Any<CancellationToken>())
            .Returns(Task.CompletedTask);

        // Act
        await _sut.AssignRoleAsync(userId, roleName, CancellationToken.None);

        // Assert
        await _getJson.Received(1).GetJsonAsync(
            Arg.Is<string>(s => s.Contains($"/roles/{Uri.EscapeDataString(roleName)}")),
            Arg.Any<CancellationToken>());

        await _postJson.Received(1).PostJsonAsync(
            Arg.Is<string>(s => s.Contains($"/users/{Uri.EscapeDataString(userId)}/role-mappings/realm")),
            Arg.Any<object>(),
            Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task AssignRoleAsync_ShouldThrow_When_GetJsonThrowsNotFound()
    {
        // Arrange
        var userId = "u1";
        var roleName = "admin";
        _getJson.GetJsonAsync(Arg.Any<string>(), Arg.Any<CancellationToken>())
            .Returns<Task<JsonElement>>(_ => throw new HttpRequestException("not found", null, System.Net.HttpStatusCode.NotFound));

        // Act
        Func<Task> act = () => _sut.AssignRoleAsync(userId, roleName, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<HttpRequestException>();
    }
}
