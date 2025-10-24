using System.Net;
using ETL.Application.Abstractions.UserServices;
using ETL.Application.Common;
using MediatR;

namespace ETL.Application.User.ChangeRole;

public record ChangeUserRoleCommand(string UserId, string Role) : IRequest<Result>;

public sealed class ChangeUserRoleCommandHandler : IRequestHandler<ChangeUserRoleCommand, Result>
{
    private readonly IOAuthRoleRemover _roleRemover;
    private readonly IOAuthRoleAssigner _roleAssigner;

    public ChangeUserRoleCommandHandler(IOAuthRoleRemover roleRemover, IOAuthRoleAssigner roleAssigner)
    {
        _roleRemover = roleRemover ?? throw new ArgumentNullException(nameof(roleRemover));
        _roleAssigner = roleAssigner ?? throw new ArgumentNullException(nameof(roleAssigner));
    }

    public async Task<Result> Handle(ChangeUserRoleCommand request, CancellationToken cancellationToken)
    {
        try
        {
            await _roleRemover.RemoveAllRealmRolesAsync(request.UserId, cancellationToken);

            await _roleAssigner.AssignRoleAsync(request.UserId, request.Role, cancellationToken);

            return Result.Success();
        }
        catch (HttpRequestException ex) when (ex.StatusCode == HttpStatusCode.NotFound)
        {
            return Result.Failure(Error.NotFound("OAuth.NotFound", ex.Message));
        }
        catch (Exception ex)
        {
            return Result.Failure(Error.Problem("User.ChangeRole.Exception", ex.Message));
        }
    }
}
