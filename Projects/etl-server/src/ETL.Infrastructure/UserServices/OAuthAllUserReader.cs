using ETL.Application.Abstractions.UserServices;
using ETL.Application.Common.Constants;
using ETL.Application.Common.DTOs;
using ETL.Infrastructure.UserServices.Abstractions;

namespace ETL.Infrastructure.UserServices
{
    public class OAuthAllUserReader : IOAuthAllUserReader
    {
        private readonly IUserFetcher _userFetcher;
        private readonly IUserJsonMapper _userMapper;
        private readonly IUserRoleAssigner _roleAssigner;

        public OAuthAllUserReader(
            IUserFetcher userFetcher,
            IUserJsonMapper userMapper,
            IUserRoleAssigner roleAssigner)
        {
            _userFetcher = userFetcher ?? throw new ArgumentNullException(nameof(userFetcher));
            _userMapper = userMapper ?? throw new ArgumentNullException(nameof(userMapper));
            _roleAssigner = roleAssigner ?? throw new ArgumentNullException(nameof(roleAssigner));
        }

        public async Task<List<UserDto>> GetAllAsync(int? first = null, int? max = null, CancellationToken ct = default)
        {
            var raw = await _userFetcher.FetchAllUsersRawAsync(first, max, ct);

            var users = raw.Select(el => _userMapper.Map(el)).ToList();

            var roles = Role.GetAllRoles();

            await _roleAssigner.AssignRolesAsync(users, roles, ct);

            return users;
        }
    }
}
