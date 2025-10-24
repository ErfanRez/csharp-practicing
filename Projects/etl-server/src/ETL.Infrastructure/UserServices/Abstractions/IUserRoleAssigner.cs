using ETL.Application.Common.DTOs;

namespace ETL.Infrastructure.UserServices.Abstractions
{
    public interface IUserRoleAssigner
    {
        Task AssignRolesAsync(List<UserDto> users, IEnumerable<string> rolesToCheck, CancellationToken ct = default);
    }
}

