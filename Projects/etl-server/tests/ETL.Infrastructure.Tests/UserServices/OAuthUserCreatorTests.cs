using System.Net;
using ETL.Application.Common.Options;
using ETL.Application.User.Create;
using ETL.Infrastructure.OAuthClients.Abstractions;
using ETL.Infrastructure.UserServices;
using FluentAssertions;
using Microsoft.Extensions.Options;
using NSubstitute;

namespace ETL.Infrastructure.Tests.UserServices;

public class OAuthUserCreatorTests
{
    private readonly IOAuthPostJsonWithResponse _postWithResponse;
    private readonly IOptions<AuthOptions> _options;
    private readonly OAuthUserCreator _sut;

    public OAuthUserCreatorTests()
    {
        _postWithResponse = Substitute.For<IOAuthPostJsonWithResponse>();
        _options = Options.Create(new AuthOptions { Realm = "myrealm" });
        _sut = new OAuthUserCreator(_postWithResponse, _options);
    }

    [Fact]
    public void Constructor_ShouldThrow_When_PostWithResponseIsNull()
    {
        // Act
        Action act = () => new OAuthUserCreator(null!, _options);

        // Assert
        act.Should().Throw<ArgumentNullException>().WithParameterName("postWithResponse");
    }

    [Fact]
    public void Constructor_ShouldThrow_When_OptionsIsNull()
    {
        // Act
        Action act = () => new OAuthUserCreator(_postWithResponse, null!);

        // Assert
        act.Should().Throw<ArgumentNullException>().WithParameterName("options");
    }

    [Fact]
    public async Task CreateUserAsync_ShouldReturnNewUserId_When_ResponseIsCreated()
    {
        // Arrange
        var command = new CreateUserCommand("u1", null, null, null, "p1", "role");
        var response = new HttpResponseMessage(HttpStatusCode.Created)
        {
            Headers = { Location = new Uri("https://fake/users/123") }
        };

        _postWithResponse.PostJsonForResponseAsync(Arg.Any<string>(), Arg.Any<object>(), Arg.Any<CancellationToken>())
            .Returns(Task.FromResult(response));

        // Act
        var result = await _sut.CreateUserAsync(command);

        // Assert
        result.Should().Be("123");
    }

    [Fact]
    public async Task CreateUserAsync_ShouldThrowInvalidOperationException_When_LocationHeaderMissing()
    {
        // Arrange
        var command = new CreateUserCommand("u1", null, null, null, "p1", "role");
        var response = new HttpResponseMessage(HttpStatusCode.Created); // no Location

        _postWithResponse.PostJsonForResponseAsync(Arg.Any<string>(), Arg.Any<object>(), Arg.Any<CancellationToken>())
            .Returns(Task.FromResult(response));

        // Act
        Func<Task> act = () => _sut.CreateUserAsync(command);

        // Assert
        await act.Should().ThrowAsync<InvalidOperationException>();
    }

    [Fact]
    public async Task CreateUserAsync_ShouldThrowHttpRequestException_When_StatusNotSuccessful()
    {
        // Arrange
        var command = new CreateUserCommand("u1", null, null, null, "p1", "role");
        var response = new HttpResponseMessage(HttpStatusCode.BadRequest)
        {
            Content = new StringContent("bad request")
        };

        _postWithResponse.PostJsonForResponseAsync(Arg.Any<string>(), Arg.Any<object>(), Arg.Any<CancellationToken>())
            .Returns(Task.FromResult(response));

        // Act
        Func<Task> act = () => _sut.CreateUserAsync(command);

        // Assert
        await act.Should().ThrowAsync<HttpRequestException>();
    }
}
