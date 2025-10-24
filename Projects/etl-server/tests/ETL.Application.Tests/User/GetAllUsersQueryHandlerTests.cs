using System.Net;
using ETL.Application.Abstractions.UserServices;
using ETL.Application.Common.DTOs;
using ETL.Application.User;
using FluentAssertions;
using NSubstitute;

namespace ETL.Application.Tests.User;

public class GetAllUsersQueryHandlerTests
{
    private readonly IOAuthAllUserReader _allUserReader;
    private readonly GetAllUsersQueryHandler _sut;

    public GetAllUsersQueryHandlerTests()
    {
        _allUserReader = Substitute.For<IOAuthAllUserReader>();
        _sut = new GetAllUsersQueryHandler(_allUserReader);
    }

    [Fact]
    public void Constructor_ShouldThrow_WhenAllUserReaderIsNull()
    {
        // Act
        Action act = () => new GetAllUsersQueryHandler(null!);

        // Assert
        act.Should().Throw<ArgumentNullException>().WithParameterName("allUserReader");
    }

    [Fact]
    public async Task Handle_ShouldReturnNotFoundFailure_WhenAllUserReaderThrowsNotFoundHttpRequestException()
    {
        // Arrange
        var query = new GetAllUsersQuery(0, 10);
        _allUserReader
            .GetAllAsync(Arg.Any<int?>(), Arg.Any<int?>(), Arg.Any<CancellationToken>())
            .Returns<Task<List<UserDto>>>(_ => throw new HttpRequestException("not found", null, HttpStatusCode.NotFound));

        // Act
        var result = await _sut.Handle(query, CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Code.Should().Be("OAuth.NotFound");
    }

    [Fact]
    public async Task Handle_ShouldReturnProblemFailure_WhenAllUserReaderThrowsGeneralException()
    {
        // Arrange
        var query = new GetAllUsersQuery(0, 10);
        _allUserReader
            .GetAllAsync(Arg.Any<int?>(), Arg.Any<int?>(), Arg.Any<CancellationToken>())
            .Returns<Task<List<UserDto>>>(_ => throw new Exception("boom"));

        // Act
        var result = await _sut.Handle(query, CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Code.Should().Be("User.GetAll.Failed");
    }

    [Fact]
    public async Task Handle_ShouldReturnEmpty_WhenNoUsers()
    {
        // Arrange
        var query = new GetAllUsersQuery(0, 10);
        _allUserReader.GetAllAsync(0, 10, Arg.Any<CancellationToken>())
            .Returns(Task.FromResult(new List<UserDto>()));

        // Act
        var result = await _sut.Handle(query, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().BeEmpty();
    }

    [Fact]
    public async Task Handle_ShouldReturnOnlyUsersWithRole_WhenSomeUsersHaveRoles()
    {
        // Arrange
        var users = new List<UserDto>
        {
            new UserDto { Id = "u1", Username = "user1", Email = "", FirstName = "", LastName = "", Role = "Admin"},
            new UserDto { Id = "u2", Username = "user2", Email = "", FirstName = "", LastName = "", Role = null}
        };
        var expected = new List<UserDto>
        {
            new UserDto { Id = "u1", Username = "user1", Email = "", FirstName = "", LastName = "", Role = "Admin"}
        };

        _allUserReader.GetAllAsync(0, 10, Arg.Any<CancellationToken>())
            .Returns(Task.FromResult(users));

        var query = new GetAllUsersQuery(0, 10);

        // Act
        var result = await _sut.Handle(query, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().BeEquivalentTo(expected);
    }
}
