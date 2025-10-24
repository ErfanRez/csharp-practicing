using System.Net;
using ETL.Application.Abstractions.UserServices;
using ETL.Application.Common;
using MediatR;
using Microsoft.Extensions.Logging;

namespace ETL.Application.User.Edit;

public record EditUserCommand(
    string UserId,
    string? Username,
    string? Email,
    string? FirstName,
    string? LastName
) : IRequest<Result>;

public sealed class EditUserCommandHandler : IRequestHandler<EditUserCommand, Result>
{
    private readonly IOAuthUserUpdater _userUpdater;

    public EditUserCommandHandler(IOAuthUserUpdater userUpdater, ILogger<EditUserCommandHandler> logger)
    {
        _userUpdater = userUpdater ?? throw new ArgumentNullException(nameof(userUpdater));
    }

    public async Task<Result> Handle(EditUserCommand request, CancellationToken cancellationToken)
    {
        try
        {
            await _userUpdater.UpdateUserAsync(request, cancellationToken);

            return Result.Success();
        }
        catch (HttpRequestException ex) when (ex.StatusCode == HttpStatusCode.NotFound)
        {
            return Result.Failure(Error.NotFound("OAuth.UserNotFound", ex.Message));
        }
        catch (Exception ex)
        {
            return Result.Failure(Error.Problem("User.Edit.Failed", ex.Message));
        }
    }
}
