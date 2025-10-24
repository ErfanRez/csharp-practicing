using System.Text.Json;
using ETL.Application.Common.Options;
using ETL.Infrastructure.OAuthClients.Abstractions;
using ETL.Infrastructure.UserServices;
using FluentAssertions;
using Microsoft.Extensions.Options;
using NSubstitute;

namespace ETL.Infrastructure.Tests.UserServices;

public class OAuthUserDeleterTests
{
    private readonly IOAuthGetJson _getJson;
    private readonly IOAuthDeleteJson _deleteJson;
    private readonly IOptions<AuthOptions> _options;
    private readonly OAuthUserDeleter _sut;

    public OAuthUserDeleterTests()
    {
        _getJson = Substitute.For<IOAuthGetJson>();
        _deleteJson = Substitute.For<IOAuthDeleteJson>();
        _options = Options.Create(new AuthOptions { Realm = "myrealm" });
        _sut = new OAuthUserDeleter(_getJson, _deleteJson, _options);
    }

    [Fact]
    public void Constructor_ShouldThrow_When_GetJsonIsNull()
    {
        // Act
        Action act = () => new OAuthUserDeleter(null!, _deleteJson, _options);

        // Assert
        act.Should().Throw<ArgumentNullException>().WithParameterName("getJson");
    }

    [Fact]
    public void Constructor_ShouldThrow_When_DeleteIsNull()
    {
        // Act
        Action act = () => new OAuthUserDeleter(_getJson, null!, _options);

        // Assert
        act.Should().Throw<ArgumentNullException>().WithParameterName("delete");
    }

    [Fact]
    public void Constructor_ShouldThrow_When_OptionsIsNull()
    {
        // Act
        Action act = () => new OAuthUserDeleter(_getJson, _deleteJson, null!);

        // Assert
        act.Should().Throw<ArgumentNullException>().WithParameterName("options");
    }

    [Fact]
    public async Task DeleteUserAsync_ShouldCallDelete_When_GetJsonSucceeds()
    {
        // Arrange
        var userId = "u1";
        var json = JsonDocument.Parse("{\"id\":\"u1\"}").RootElement;
        _getJson.GetJsonAsync(Arg.Any<string>(), Arg.Any<CancellationToken>())
            .Returns(Task.FromResult(json));
        _deleteJson.DeleteJsonAsync(Arg.Any<string>(), Arg.Any<object?>(), Arg.Any<CancellationToken>())
            .Returns(Task.CompletedTask);

        // Act
        await _sut.DeleteUserAsync(userId);

        // Assert
        await _deleteJson.Received(1).DeleteJsonAsync(
            Arg.Is<string>(s => s.Contains($"/users/{Uri.EscapeDataString(userId)}")),
            Arg.Is<object?>(o => o == null),
            Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task DeleteUserAsync_ShouldThrowNotFound_When_GetJsonThrowsNotFound()
    {
        // Arrange
        var userId = "u1";
        _getJson.GetJsonAsync(Arg.Any<string>(), Arg.Any<CancellationToken>())
            .Returns<Task<JsonElement>>(_ => throw new HttpRequestException("not found", null, System.Net.HttpStatusCode.NotFound));

        // Act
        Func<Task> act = () => _sut.DeleteUserAsync(userId);

        // Assert
        await act.Should().ThrowAsync<HttpRequestException>();
    }

    [Fact]
    public async Task DeleteUserAsync_ShouldThrow_When_DeleteThrows()
    {
        // Arrange
        var userId = "u1";
        var json = JsonDocument.Parse("{\"id\":\"u1\"}").RootElement;
        _getJson.GetJsonAsync(Arg.Any<string>(), Arg.Any<CancellationToken>())
            .Returns(Task.FromResult(json));
        _deleteJson.DeleteJsonAsync(Arg.Any<string>(), Arg.Any<object?>(), Arg.Any<CancellationToken>())
            .Returns<Task>(_ => throw new InvalidOperationException("boom"));

        // Act
        Func<Task> act = () => _sut.DeleteUserAsync(userId);

        // Assert
        await act.Should().ThrowAsync<InvalidOperationException>().WithMessage("*boom*");
    }
}
