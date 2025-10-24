using System.Net;
using ETL.Application.Abstractions.UserServices;
using ETL.Application.Common;
using MediatR;
using Microsoft.Extensions.Logging;

namespace ETL.Application.User.Delete;

public record DeleteUserCommand(string UserId) : IRequest<Result>;

public sealed class DeleteUserCommandHandler : IRequestHandler<DeleteUserCommand, Result>
{
    private readonly IOAuthUserDeleter _userDeleter;

    public DeleteUserCommandHandler(IOAuthUserDeleter userDeleter, ILogger<DeleteUserCommandHandler> logger)
    {
        _userDeleter = userDeleter ?? throw new ArgumentNullException(nameof(userDeleter));
    }

    public async Task<Result> Handle(DeleteUserCommand request, CancellationToken cancellationToken)
    {
        try
        {
            await _userDeleter.DeleteUserAsync(request.UserId, cancellationToken);
            return Result.Success();
        }
        catch (HttpRequestException ex) when (ex.StatusCode == HttpStatusCode.NotFound)
        {
            return Result.Failure(Error.NotFound("OAuth.UserNotFound", $"User '{request.UserId}' not found."));
        }
        catch (Exception ex)
        {
            return Result.Failure(Error.Problem("User.Delete.Failed", ex.Message));
        }
    }
}