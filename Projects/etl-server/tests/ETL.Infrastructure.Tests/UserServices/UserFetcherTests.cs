using System.Text.Json;
using ETL.Application.Common.Options;
using ETL.Infrastructure.OAuthClients.Abstractions;
using ETL.Infrastructure.UserServices;
using FluentAssertions;
using Microsoft.Extensions.Options;
using NSubstitute;

namespace ETL.Infrastructure.Tests.UserServices;

public class UserFetcherTests
{
    private readonly IOAuthGetJsonArray _getArray;
    private readonly IOptions<AuthOptions> _options;
    private readonly UserFetcher _sut;

    public UserFetcherTests()
    {
        _getArray = Substitute.For<IOAuthGetJsonArray>();
        var opt = new AuthOptions { Realm = "my realm" };
        _options = Options.Create(opt);
        _sut = new UserFetcher(_getArray, _options);
    }

    [Fact]
    public void Constructor_ShouldThrow_WhenGetArrayIsNull()
    {
        // Act
        Action act = () => new UserFetcher(null!, _options);

        // Assert
        act.Should().Throw<ArgumentNullException>().WithParameterName("getArray");
    }

    [Fact]
    public void Constructor_ShouldThrow_WhenOptionsIsNull()
    {
        // Act
        Action act = () => new UserFetcher(_getArray, null!);

        // Assert
        act.Should().Throw<ArgumentNullException>().WithParameterName("options");
    }

    [Fact]
    public async Task FetchAllUsersRawAsync_ShouldReturnListAndIncludeQueryParams_WhenProvided()
    {
        // Arrange
        var expected = new List<JsonElement>
        {
            JsonDocument.Parse("{\"id\":\"1\"}").RootElement
        };

        _getArray.GetJsonArrayAsync(Arg.Any<string>(), Arg.Any<CancellationToken>())
            .Returns(Task.FromResult(expected));

        // Act
        var result = await _sut.FetchAllUsersRawAsync(first: 5, max: 10, ct: CancellationToken.None);

        // Assert
        result.Should().BeEquivalentTo(expected);
        await _getArray.Received(1).GetJsonArrayAsync(Arg.Is<string>(s => s.Contains("first=5") && s.Contains("max=10") && s.Contains(Uri.EscapeDataString("my realm"))), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task FetchAllUsersRawAsync_ShouldReturnEmptyList_WhenGetArrayReturnsNull()
    {
        // Arrange
        _getArray.GetJsonArrayAsync(Arg.Any<string>(), Arg.Any<CancellationToken>())
            .Returns(Task.FromResult<List<JsonElement>>(null));

        var expected = new List<JsonElement>();

        // Act
        var result = await _sut.FetchAllUsersRawAsync();

        // Assert
        result.Should().BeEquivalentTo(expected);
    }
}
