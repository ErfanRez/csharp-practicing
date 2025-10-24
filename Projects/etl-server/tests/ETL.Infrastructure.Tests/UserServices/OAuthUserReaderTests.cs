using System.Net;
using System.Text.Json;
using ETL.Application.Common.Options;
using ETL.Infrastructure.OAuthClients.Abstractions;
using ETL.Infrastructure.UserServices;
using FluentAssertions;
using Microsoft.Extensions.Options;
using NSubstitute;

namespace ETL.Infrastructure.Tests.UserServices;

public class OAuthUserReaderTests
{
    private readonly IOAuthGetJson _getJson;
    private readonly IOptions<AuthOptions> _options;
    private readonly OAuthUserReader _sut;

    public OAuthUserReaderTests()
    {
        _getJson = Substitute.For<IOAuthGetJson>();
        _options = Options.Create(new AuthOptions { Realm = "myrealm" });
        _sut = new OAuthUserReader(_getJson, _options);
    }

    [Fact]
    public void Constructor_ShouldThrowArgumentNullException_WhenGetJsonIsNull()
    {
        // Act
        Action act = () => new OAuthUserReader(null!, _options);

        // Assert
        act.Should().Throw<ArgumentNullException>().WithParameterName("getJson");
    }

    [Fact]
    public void Constructor_ShouldThrowArgumentNullException_WhenOptionsIsNull()
    {
        // Act
        Action act = () => new OAuthUserReader(_getJson, null!);

        // Assert
        act.Should().Throw<ArgumentNullException>().WithParameterName("options");
    }

    [Fact]
    public async Task GetByIdAsync_ShouldReturnMappedUser_WhenResponseIsSuccessful()
    {
        // Arrange
        var userId = "u1";
        var json = JsonDocument.Parse("{\"id\":\"u1\",\"username\":\"user1\",\"email\":\"e1\",\"firstName\":\"f1\",\"lastName\":\"l1\"}").RootElement;
        var expected = new ETL.Application.Common.DTOs.UserDto
        {
            Id = "u1",
            Username = "user1",
            Email = "e1",
            FirstName = "f1",
            LastName = "l1",
            Role = null
        };

        _getJson.GetJsonAsync(Arg.Any<string>(), Arg.Any<CancellationToken>())
            .Returns(Task.FromResult(json));

        // Act
        var result = await _sut.GetByIdAsync(userId);

        // Assert
        result.Should().BeEquivalentTo(expected);
        await _getJson.Received(1).GetJsonAsync(Arg.Is<string>(s => s.Contains($"/users/{Uri.EscapeDataString(userId)}") && s.Contains(Uri.EscapeDataString("myrealm"))), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task GetByIdAsync_ShouldHandleMissingProperties()
    {
        // Arrange
        var userId = "u1";
        var json = JsonDocument.Parse("{\"id\":\"u1\"}").RootElement;
        var expected = new ETL.Application.Common.DTOs.UserDto
        {
            Id = "u1",
            Username = null,
            Email = null,
            FirstName = null,
            LastName = null,
            Role = null
        };

        _getJson.GetJsonAsync(Arg.Any<string>(), Arg.Any<CancellationToken>())
            .Returns(Task.FromResult(json));

        // Act
        var result = await _sut.GetByIdAsync(userId);

        // Assert
        result.Should().BeEquivalentTo(expected);
    }

    [Fact]
    public async Task GetByIdAsync_ShouldThrow_When_GetJsonThrowsHttpRequestException()
    {
        // Arrange
        var userId = "u1";
        _getJson.GetJsonAsync(Arg.Any<string>(), Arg.Any<CancellationToken>())
            .Returns<Task<JsonElement>>(_ => throw new HttpRequestException("not found", null, HttpStatusCode.NotFound));

        // Act
        Func<Task> act = () => _sut.GetByIdAsync(userId);

        // Assert
        await act.Should().ThrowAsync<HttpRequestException>();
    }
}
