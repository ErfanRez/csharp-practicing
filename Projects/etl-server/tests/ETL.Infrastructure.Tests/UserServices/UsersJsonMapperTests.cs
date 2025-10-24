using System.Text.Json;
using ETL.Application.Common.DTOs;
using ETL.Infrastructure.UserServices;
using FluentAssertions;

namespace ETL.Infrastructure.Tests.UserServices;

public class UserJsonMapperTests
{
    private readonly UserJsonMapper _sut;

    public UserJsonMapperTests()
    {
        _sut = new UserJsonMapper();
    }

    [Fact]
    public void Map_ShouldMapProperties_WhenJsonHasProperties()
    {
        // Arrange
        var json = JsonDocument.Parse("{\"id\":\"1\",\"username\":\"u1\",\"email\":\"e1\",\"firstName\":\"f1\",\"lastName\":\"l1\"}").RootElement;
        var expected = new UserDto
        {
            Id = "1",
            Username = "u1",
            Email = "e1",
            FirstName = "f1",
            LastName = "l1",
            Role = null
        };

        // Act
        var result = _sut.Map(json);

        // Assert
        result.Should().BeEquivalentTo(expected);
    }

    [Fact]
    public void Map_ShouldReturnNullProperties_WhenJsonMissingProperties()
    {
        // Arrange
        var json = JsonDocument.Parse("{}").RootElement;
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
        var result = _sut.Map(json);

        // Assert
        result.Should().BeEquivalentTo(expected);
    }
}
