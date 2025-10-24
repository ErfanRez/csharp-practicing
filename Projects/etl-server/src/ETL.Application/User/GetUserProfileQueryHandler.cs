using System.Security.Claims;
using ETL.Application.Common.DTOs;
using MediatR;

namespace ETL.Application.User;

public record GetUserProfileQuery(ClaimsPrincipal User) : IRequest<UserDto>;

public sealed class GetUserProfileQueryHandler : IRequestHandler<GetUserProfileQuery, UserDto>
{
    public Task<UserDto> Handle(GetUserProfileQuery request, CancellationToken cancellationToken)
    {
        var user = request.User;

        var userProfile = new UserDto
        {
            Id = user.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value,
            Username = user.Claims.FirstOrDefault(c => c.Type == "preferred_username")?.Value,
            Email = user.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value,
            FirstName = user.Claims.FirstOrDefault(c => c.Type == ClaimTypes.GivenName)?.Value,
            LastName = user.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Surname)?.Value,
            Role = user.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Role)?.Value,
        };

        return Task.FromResult(userProfile);
    }
}
