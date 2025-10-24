using ETL.Infrastructure.Repositories;
using FluentAssertions;

namespace ETL.Infrastructure.Tests;

public class IdentifierSanitizerTests
{
    private readonly IdentifierSanitizer _sut = new IdentifierSanitizer();

    [Fact]
    public void Sanitize_ShouldReturnQuotedAlphanumeric_When_InputHasSpecialChars()
    {
        // Arrange
        var input = "col-name$with#chars";

        // Act
        var result = _sut.Sanitize(input);

        // Assert
        result.Should().StartWith("\"").And.EndWith("\"");
        result.Should().Contain("colnamewithchars");
    }

    [Fact]
    public void Sanitize_ShouldTruncate_When_ResultLongerThan63()
    {
        // Arrange
        var input = new string('a', 100);

        // Act
        var result = _sut.Sanitize(input);

        // Assert
        result.Length.Should().BeLessThanOrEqualTo(65);
    }

    [Fact]
    public void Sanitize_ShouldThrow_When_InputIsWhitespace()
    {
        // Act
        Action act = () => _sut.Sanitize("   ");

        // Assert
        act.Should().Throw<ArgumentException>();
    }

    [Fact]
    public void Sanitize_ShouldThrow_When_AllCharsRemoved()
    {
        // Act
        Action act = () => _sut.Sanitize("!!!@@@");

        // Assert
        act.Should().Throw<ArgumentException>();
    }
}
