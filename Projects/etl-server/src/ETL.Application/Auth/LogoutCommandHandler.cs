using ETL.Application.Abstractions.Security;
using ETL.Application.Common;
using MediatR;

namespace ETL.Application.Auth;

public record LogoutCommand(string? AccessToken, string? RefreshToken) : IRequest<Result>;

public sealed class LogoutCommandHandler : IRequestHandler<LogoutCommand, Result>
{
    private readonly IAuthLogoutService _logoutService;

    public LogoutCommandHandler(IAuthLogoutService logoutService)
    {
        _logoutService = logoutService ?? throw new ArgumentNullException(nameof(logoutService));
    }

    public async Task<Result> Handle(LogoutCommand request, CancellationToken cancellationToken)
    {
        try
        {
            await _logoutService.LogoutAsync(request.AccessToken, request.RefreshToken, cancellationToken);
            return Result.Success();
        }
        catch (Exception ex)
        {
            return Result.Failure(Error.Problem("Auth.LogOut.Failed", ex.Message));
        }
    }
}
