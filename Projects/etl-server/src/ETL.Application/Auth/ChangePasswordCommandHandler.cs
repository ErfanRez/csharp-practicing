using System.Security.Claims;
using ETL.Application.Abstractions.Security;
using ETL.Application.Auth.DTOs;
using ETL.Application.Common;
using MediatR;

namespace ETL.Application.Auth;

public record ChangePasswordCommand(ChangePasswordDto Request, ClaimsPrincipal User) : IRequest<Result>;

public sealed class ChangePasswordCommandHandler : IRequestHandler<ChangePasswordCommand, Result>
{
    private readonly IAuthCredentialValidator _credentialValidator;
    private readonly IAuthRestPasswordService _restPasswordService;

    public ChangePasswordCommandHandler(IAuthCredentialValidator credentialValidator, IAuthRestPasswordService restPasswordService)
    {
        _credentialValidator = credentialValidator ?? throw new ArgumentNullException(nameof(credentialValidator));
        _restPasswordService = restPasswordService ?? throw new ArgumentNullException(nameof(restPasswordService));
    }

    public async Task<Result> Handle(ChangePasswordCommand request, CancellationToken cancellationToken)
    {
        var dto = request.Request;
        if (dto is null)
            return Result.Failure(Error.Validation("Auth.InvalidRequest", "Request is missing"));

        if (dto.NewPassword != dto.ConfirmPassword)
            return Result.Failure(Error.Validation("Auth.PasswordMismatch", "New password and confirmation do not match"));

        var user = request.User;
        var userId = user.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
        var username = user.Claims.FirstOrDefault(c => c.Type == "preferred_username")?.Value;

        if (string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(username))
            return Result.Failure(Error.NotFound("Auth.UserNotFound", "User identity not found"));

        var valid = await _credentialValidator.ValidateCredentialsAsync(username, dto.CurrentPassword, cancellationToken);
        if (!valid)
            return Result.Failure(Error.Validation("Auth.InvalidCurrentPassword", "The current password is incorrect"));

        try
        {
            await _restPasswordService.ResetPasswordAsync(userId, dto.NewPassword, cancellationToken);
            return Result.Success();
        }
        catch (Exception ex)
        {
            return Result.Failure(Error.Problem("Auth.PasswordChange.Failed", $"Failed to change password: {ex.Message}"));
        }
    }
}
