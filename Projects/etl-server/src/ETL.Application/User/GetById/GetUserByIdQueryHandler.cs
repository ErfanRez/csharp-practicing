using System.Net;
using ETL.Application.Abstractions.UserServices;
using ETL.Application.Common;
using ETL.Application.Common.DTOs;
using MediatR;

namespace ETL.Application.User.GetById;

public record GetUserByIdQuery(string UserId) : IRequest<Result<UserDto>>;

public sealed class GetUserByIdQueryHandler : IRequestHandler<GetUserByIdQuery, Result<UserDto>>
{
    private readonly IOAuthUserReader _userReader;
    private readonly IOAuthUserRoleGetter _roleGetter;

    public GetUserByIdQueryHandler(IOAuthUserReader userReader, IOAuthUserRoleGetter roleGetter)
    {
        _userReader = userReader ?? throw new ArgumentNullException(nameof(userReader));
        _roleGetter = roleGetter ?? throw new ArgumentNullException(nameof(roleGetter));
    }

    public async Task<Result<UserDto>> Handle(GetUserByIdQuery request, CancellationToken cancellationToken)
    {
        UserDto user;
        try
        {
            user = await _userReader.GetByIdAsync(request.UserId, cancellationToken);
        }
        catch (HttpRequestException ex) when (ex.StatusCode == HttpStatusCode.NotFound)
        {
            return Result.Failure<UserDto>(Error.NotFound("User.NotFound", $"User '{request.UserId}' was not found."));
        }
        catch (Exception ex)
        {
            return Result.Failure<UserDto>(Error.Problem("User.GetById.Unexpected", ex.Message));
        }

        try
        {
            var role = await _roleGetter.GetRoleForUserAsync(request.UserId, cancellationToken);
            user.Role = role;
        }
        catch (Exception ex)
        {
            return Result.Failure<UserDto>(Error.Problem("User.GetById.Exception", ex.Message));
        }

        return Result.Success(user);
    }
}
