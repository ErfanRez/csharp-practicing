using System.Net;
using ETL.Application.Abstractions.UserServices;
using ETL.Application.Common;
using MediatR;

namespace ETL.Application.User.Create;

public record CreateUserCommand(
    string Username,
    string? Email,
    string? FirstName,
    string? LastName,
    string Password,
    string Role
) : IRequest<Result<string>>;

public sealed class CreateUserCommandHandler : IRequestHandler<CreateUserCommand, Result<string>>
{
    private readonly IOAuthUserCreator _userCreator;
    private readonly IOAuthRoleAssigner _roleAssigner;

    public CreateUserCommandHandler(IOAuthUserCreator userCreator, IOAuthRoleAssigner roleAssigner)
    {
        _userCreator = userCreator ?? throw new ArgumentNullException(nameof(userCreator));
        _roleAssigner = roleAssigner ?? throw new ArgumentNullException(nameof(roleAssigner));
    }

    public async Task<Result<string>> Handle(CreateUserCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var newUserId = await _userCreator.CreateUserAsync(request, cancellationToken);

            await _roleAssigner.AssignRoleAsync(newUserId, request.Role, cancellationToken);

            return Result.Success(newUserId);
        }
        catch (HttpRequestException ex) when (ex.StatusCode == HttpStatusCode.Conflict)
        {

            return Result.Failure<string>(Error.Conflict("OAuth.User.Exists", "User already exists."));
        }
        catch (Exception ex)
        {

            return Result.Failure<string>(Error.Problem("User.Create.Failed", ex.Message));
        }
    }
}