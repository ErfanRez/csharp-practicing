using ETL.Application.Common.Options;
using ETL.Application.User.Edit;
using ETL.Infrastructure.OAuthClients.Abstractions;
using ETL.Infrastructure.UserServices;
using FluentAssertions;
using Microsoft.Extensions.Options;
using NSubstitute;

namespace ETL.Infrastructure.Tests.UserServices;

public class OAuthUserUpdaterTests
{
    private readonly IOAuthPutJson _putJson;
    private readonly IOptions<AuthOptions> _options;
    private readonly OAuthUserUpdater _sut;

    public OAuthUserUpdaterTests()
    {
        _putJson = Substitute.For<IOAuthPutJson>();
        _options = Options.Create(new AuthOptions { Realm = "myrealm" });
        _sut = new OAuthUserUpdater(_putJson, _options);
    }

    [Fact]
    public void Constructor_ShouldThrow_When_PutJsonIsNull()
    {
        // Act
        Action act = () => new OAuthUserUpdater(null!, _options);

        // Assert
        act.Should().Throw<ArgumentNullException>().WithParameterName("putJson");
    }

    [Fact]
    public void Constructor_ShouldThrow_When_OptionsIsNull()
    {
        // Act
        Action act = () => new OAuthUserUpdater(_putJson, null!);

        // Assert
        act.Should().Throw<ArgumentNullException>().WithParameterName("options");
    }

    [Fact]
    public async Task UpdateUserAsync_ShouldNotCallPut_When_NoFieldsProvided()
    {
        // Arrange
        var cmd = new EditUserCommand("u1", null, null, null, null);

        // Act
        await _sut.UpdateUserAsync(cmd);

        // Assert
        await _putJson.DidNotReceiveWithAnyArgs().PutJsonAsync(default!, default!, default);
    }

    [Fact]
    public async Task UpdateUserAsync_ShouldCallPutJson_WithCorrectPathAndPayload_When_FieldsProvided()
    {
        // Arrange
        var cmd = new EditUserCommand("user@id", "alice", "", "", "Smith");
        string? capturedPath = null;
        Dictionary<string, object?>? capturedPayload = null;

        _putJson.PutJsonAsync(Arg.Any<string>(), Arg.Any<Dictionary<string, object?>>(), Arg.Any<CancellationToken>())
            .Returns(Task.CompletedTask)
            .AndDoes(ci =>
            {
                capturedPath = ci.ArgAt<string>(0);
                capturedPayload = ci.ArgAt<Dictionary<string, object?>>(1);
            });

        // Act
        await _sut.UpdateUserAsync(cmd);

        // Assert
        capturedPath.Should().Be("/admin/realms/myrealm/users/user%40id");
        capturedPayload.Should().ContainKey("username").WhoseValue.Should().Be("alice");
    }

    [Fact]
    public async Task UpdateUserAsync_ShouldThrow_When_PutJsonThrows()
    {
        // Arrange
        var cmd = new EditUserCommand("u1", "a", null, null, null);
        _putJson.PutJsonAsync(Arg.Any<string>(), Arg.Any<Dictionary<string, object?>>(), Arg.Any<CancellationToken>())
            .Returns<Task>(_ => throw new InvalidOperationException("boom"));

        // Act
        Func<Task> act = () => _sut.UpdateUserAsync(cmd);

        // Assert
        await act.Should().ThrowAsync<InvalidOperationException>().WithMessage("*boom*");
    }
}
