using System.Net;
using ETL.Application.Abstractions.UserServices;
using ETL.Application.Common.DTOs;
using ETL.Application.User.GetById;
using FluentAssertions;
using NSubstitute;

namespace ETL.Application.Tests.User;

public class GetUserByIdQueryHandlerTests
{
    private readonly IOAuthUserReader _userReader;
    private readonly IOAuthUserRoleGetter _roleGetter;
    private readonly GetUserByIdQueryHandler _sut;

    public GetUserByIdQueryHandlerTests()
    {
        _userReader = Substitute.For<IOAuthUserReader>();
        _roleGetter = Substitute.For<IOAuthUserRoleGetter>();
        _sut = new GetUserByIdQueryHandler(_userReader, _roleGetter);
    }

    [Fact]
    public async Task Handle_ShouldReturnNotFoundFailure_When_UserReaderThrowsNotFoundHttpRequestException()
    {
        // Arrange
        var query = new GetUserByIdQuery("u1");
        _userReader.GetByIdAsync("u1", Arg.Any<CancellationToken>())
            .Returns<Task<UserDto>>(_ => throw new HttpRequestException("not found", null, HttpStatusCode.NotFound));

        // Act
        var result = await _sut.Handle(query, CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Code.Should().Be("User.NotFound");
    }

    [Fact]
    public async Task Handle_ShouldReturnProblemFailure_When_UserReaderThrowsGeneralException()
    {
        // Arrange
        var query = new GetUserByIdQuery("u1");
        _userReader.GetByIdAsync("u1", Arg.Any<CancellationToken>())
            .Returns<Task<UserDto>>(_ => throw new Exception("boom"));

        // Act
        var result = await _sut.Handle(query, CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Code.Should().Be("User.GetById.Unexpected");
    }

    [Fact]
    public async Task Handle_ShouldReturnFailure_When_RoleGetterThrowsException()
    {
        // Arrange
        var query = new GetUserByIdQuery("u1");
        var user = new UserDto { Id = "u1", Username = "test", FirstName = "", LastName = "", Email = "" };
        _userReader.GetByIdAsync("u1", Arg.Any<CancellationToken>())
            .Returns(Task.FromResult(user));

        _roleGetter.GetRoleForUserAsync("u1", Arg.Any<CancellationToken>())
            .Returns<Task<string?>>(_ => throw new Exception("role fail"));

        // Act
        var result = await _sut.Handle(query, CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Code.Should().Be("User.GetById.Exception");
    }

    [Fact]
    public async Task Handle_ShouldReturnSuccess_WithUserAndRole_When_ReaderAndRoleGetterSucceed()
    {
        // Arrange
        var query = new GetUserByIdQuery("u1");
        var user = new UserDto { Id = "u1", Username = "test", FirstName = "", LastName = "", Email = "" };
        _userReader.GetByIdAsync("u1", Arg.Any<CancellationToken>())
            .Returns(Task.FromResult(user));

        _roleGetter.GetRoleForUserAsync("u1", Arg.Any<CancellationToken>())
            .Returns(Task.FromResult<string?>("Admin"));

        // Act
        var result = await _sut.Handle(query, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Role.Should().Be("Admin");
    }

    [Fact]
    public void Constructor_ShouldThrow_When_UserReaderIsNull()
    {
        // Act
        Action act = () => new GetUserByIdQueryHandler(null!, _roleGetter);

        // Assert
        act.Should().Throw<ArgumentNullException>().WithParameterName("userReader");
    }

    [Fact]
    public void Constructor_ShouldThrow_When_RoleGetterIsNull()
    {
        // Act
        Action act = () => new GetUserByIdQueryHandler(_userReader, null!);

        // Assert
        act.Should().Throw<ArgumentNullException>().WithParameterName("roleGetter");
    }
}
