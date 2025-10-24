using System.Net;
using ETL.Application.Abstractions.UserServices;
using ETL.Application.Common;
using ETL.Application.Common.DTOs;
using MediatR;

namespace ETL.Application.User;

public record GetAllUsersQuery(int? First = null, int? Max = null) : IRequest<Result<IEnumerable<UserDto>>>;

public sealed class GetAllUsersQueryHandler : IRequestHandler<GetAllUsersQuery, Result<IEnumerable<UserDto>>>
{
    private readonly IOAuthAllUserReader _allUserReader;

    public GetAllUsersQueryHandler(IOAuthAllUserReader allUserReader)
    {
        _allUserReader = allUserReader ?? throw new ArgumentNullException(nameof(allUserReader));
    }

    public async Task<Result<IEnumerable<UserDto>>> Handle(GetAllUsersQuery request, CancellationToken cancellationToken)
    {
        List<UserDto> users;

        try
        {
            users = await _allUserReader.GetAllAsync(request.First, request.Max, cancellationToken);
        }
        catch (HttpRequestException ex) when (ex.StatusCode == HttpStatusCode.NotFound)
        {
            return Result.Failure<IEnumerable<UserDto>>(Error.NotFound("OAuth.NotFound", ex.Message));
        }
        catch (Exception ex)
        {
            return Result.Failure<IEnumerable<UserDto>>(Error.Problem("User.GetAll.Failed", ex.Message));
        }

        if (users == null || users.Count == 0)
            return Result.Success<IEnumerable<UserDto>>([]);

        var withRole = users.Where(u => !string.IsNullOrEmpty(u.Role)).ToList();

        return Result.Success<IEnumerable<UserDto>>(withRole);
    }
}
