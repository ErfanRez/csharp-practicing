using System.Security.Claims;
using ETL.Application.Abstractions.Security;
using ETL.Application.Auth;
using ETL.Application.Auth.DTOs;
using FluentAssertions;
using NSubstitute;

namespace ETL.Application.Tests.Auth;

public class ChangePasswordCommandHandlerTests
{
    private readonly IAuthCredentialValidator _credentialValidator;
    private readonly IAuthRestPasswordService _resetPasswordService;
    private readonly ChangePasswordCommandHandler _sut;

    public ChangePasswordCommandHandlerTests()
    {
        _credentialValidator = Substitute.For<IAuthCredentialValidator>();
        _resetPasswordService = Substitute.For<IAuthRestPasswordService>();
        _sut = new ChangePasswordCommandHandler(_credentialValidator, _resetPasswordService);
    }

    [Fact]
    public void Constructor_ShouldThrowArgumentNullException_WhenCredentialValidatorIsNull()
    {
        // Arrange // Act
        Action act = () => new ChangePasswordCommandHandler(null!, _resetPasswordService);

        // Assert
        act.Should().Throw<ArgumentNullException>().WithParameterName("credentialValidator");
    }

    [Fact]
    public void Constructor_ShouldThrowArgumentNullException_WhenRestPasswordServiceIsNull()
    {
        // Arrange // Act
        Action act = () => new ChangePasswordCommandHandler(_credentialValidator, null!);

        // Assert
        act.Should().Throw<ArgumentNullException>().WithParameterName("restPasswordService");
    }

    private static ClaimsPrincipal CreateUser(string userId = "123", string username = "testuser")
    {
        var claims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier, userId),
            new Claim("preferred_username", username)
        };
        return new ClaimsPrincipal(new ClaimsIdentity(claims));
    }

    [Fact]
    public async Task Handle_ShouldReturnFailure_WhenRequestIsNull()
    {
        // Arrange
        var command = new ChangePasswordCommand(null!, CreateUser());

        // Act
        var result = await _sut.Handle(command, CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Code.Should().Be("Auth.InvalidRequest");
    }

    [Fact]
    public async Task Handle_ShouldReturnFailure_WhenPasswordsDoNotMatch()
    {
        // Arrange
        var dto = new ChangePasswordDto
        {
            CurrentPassword = "old",
            NewPassword = "new",
            ConfirmPassword = "different"
        };
        var command = new ChangePasswordCommand(dto, CreateUser());

        // Act
        var result = await _sut.Handle(command, CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Code.Should().Be("Auth.PasswordMismatch");
    }

    [Fact]
    public async Task Handle_ShouldReturnFailure_WhenUserClaimsMissing()
    {
        // Arrange
        var dto = new ChangePasswordDto
        {
            CurrentPassword = "old",
            NewPassword = "new",
            ConfirmPassword = "new"
        };
        var command = new ChangePasswordCommand(dto, new ClaimsPrincipal());

        // Act
        var result = await _sut.Handle(command, CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Code.Should().Be("Auth.UserNotFound");
    }

    [Fact]
    public async Task Handle_ShouldReturnFailure_WhenCurrentPasswordInvalid()
    {
        // Arrange
        var dto = new ChangePasswordDto
        {
            CurrentPassword = "wrong",
            NewPassword = "new",
            ConfirmPassword = "new"
        };
        var command = new ChangePasswordCommand(dto, CreateUser());

        _credentialValidator
            .ValidateCredentialsAsync("testuser", "wrong", Arg.Any<CancellationToken>())
            .Returns(Task.FromResult(false));

        // Act
        var result = await _sut.Handle(command, CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Code.Should().Be("Auth.InvalidCurrentPassword");
    }

    [Fact]
    public async Task Handle_ShouldReturnSuccess_WhenPasswordResetSucceeds()
    {
        // Arrange
        var dto = new ChangePasswordDto
        {
            CurrentPassword = "old",
            NewPassword = "new",
            ConfirmPassword = "new"
        };
        var command = new ChangePasswordCommand(dto, CreateUser());

        _credentialValidator
            .ValidateCredentialsAsync("testuser", "old", Arg.Any<CancellationToken>())
            .Returns(Task.FromResult(true));

        _resetPasswordService
            .ResetPasswordAsync("123", "new", Arg.Any<CancellationToken>())
            .Returns(Task.CompletedTask);

        // Act
        var result = await _sut.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        await _resetPasswordService.Received(1).ResetPasswordAsync("123", "new", Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_ShouldReturnFailure_WhenResetPasswordThrows()
    {
        // Arrange
        var dto = new ChangePasswordDto
        {
            CurrentPassword = "old",
            NewPassword = "new",
            ConfirmPassword = "new"
        };
        var command = new ChangePasswordCommand(dto, CreateUser());

        _credentialValidator
            .ValidateCredentialsAsync("testuser", "old", Arg.Any<CancellationToken>())
            .Returns(Task.FromResult(true));

        _resetPasswordService
            .ResetPasswordAsync("123", "new", Arg.Any<CancellationToken>())
            .Returns<Task>(x => throw new InvalidOperationException("boom"));

        // Act
        var result = await _sut.Handle(command, CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Code.Should().Be("Auth.PasswordChange.Failed");
    }
}
