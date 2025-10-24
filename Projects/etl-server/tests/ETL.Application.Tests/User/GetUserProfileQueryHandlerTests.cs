using System.Security.Claims;
using ETL.Application.Common.DTOs;
using ETL.Application.User;
using FluentAssertions;

namespace ETL.Application.Tests.User;

public class GetUserProfileQueryHandlerTests
{
    private readonly GetUserProfileQueryHandler _sut;

    public GetUserProfileQueryHandlerTests()
    {
        _sut = new GetUserProfileQueryHandler();
    }

    private static ClaimsPrincipal CreateUser(
        string? id = "123",
        string? username = "testuser",
        string? email = "test@mail.com",
        string? firstName = "John",
        string? lastName = "Doe",
        string? role = "User")
    {
        var claims = new List<Claim>();

        if (id != null) claims.Add(new Claim(ClaimTypes.NameIdentifier, id));
        if (username != null) claims.Add(new Claim("preferred_username", username));
        if (email != null) claims.Add(new Claim(ClaimTypes.Email, email));
        if (firstName != null) claims.Add(new Claim(ClaimTypes.GivenName, firstName));
        if (lastName != null) claims.Add(new Claim(ClaimTypes.Surname, lastName));
        if (role != null) claims.Add(new Claim(ClaimTypes.Role, role));

        return new ClaimsPrincipal(new ClaimsIdentity(claims));
    }

    [Fact]
    public async Task Handle_ShouldMapAllClaims_WhenPresent()
    {
        // Arrange
        var user = CreateUser();
        var query = new GetUserProfileQuery(user);
        var expected = new UserDto
        {
            Id = "123",
            Username = "testuser",
            Email = "test@mail.com",
            FirstName = "John",
            LastName = "Doe",
            Role = "User"
        };

        // Act
        var result = await _sut.Handle(query, default);

        // Assert
        result.Should().BeEquivalentTo(expected);
    }

    [Fact]
    public async Task Handle_ShouldReturnNullProperties_WhenClaimsMissing()
    {
        // Arrange
        var user = new ClaimsPrincipal(new ClaimsIdentity());
        var query = new GetUserProfileQuery(user);
        var expected = new UserDto
        {
            Id = null,
            Username = null,
            Email = null,
            FirstName = null,
            LastName = null,
            Role = null
        };

        // Act
        var result = await _sut.Handle(query, default);

        // Assert
        result.Should().BeEquivalentTo(expected);
    }
}
