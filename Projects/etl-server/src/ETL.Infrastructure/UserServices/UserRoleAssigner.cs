using ETL.Application.Common.DTOs;
using ETL.Infrastructure.UserServices.Abstractions;

namespace ETL.Infrastructure.UserServices
{
    public class UserRoleAssigner : IUserRoleAssigner
    {
        private readonly IUsersRoleFetcher _roleUsersFetcher;

        public UserRoleAssigner(IUsersRoleFetcher roleUsersFetcher)
        {
            _roleUsersFetcher = roleUsersFetcher ?? throw new ArgumentNullException(nameof(roleUsersFetcher));
        }

        public async Task AssignRolesAsync(List<UserDto> users, IEnumerable<string> rolesToCheck, CancellationToken ct = default)
        {
            if (users == null || users.Count == 0) return;

            var rolesList = rolesToCheck?.ToList() ?? new List<string>();
            if (rolesList.Count == 0) return;

            var roleTasks = rolesList.Select(rn => _roleUsersFetcher.FetchUsersForRoleAsync(rn, ct)).ToList();
            await Task.WhenAll(roleTasks).ConfigureAwait(false);

            var userRoleMap = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

            for (int i = 0; i < rolesList.Count; i++)
            {
                var roleName = rolesList[i];
                var roleUsers = roleTasks[i].Result;

                if (roleUsers == null) continue;

                foreach (var uel in roleUsers)
                {
                    if (uel.TryGetProperty("id", out var pId) && pId.GetString() is string userId)
                    {
                        if (!userRoleMap.ContainsKey(userId))
                            userRoleMap[userId] = roleName;
                    }
                }
            }

            foreach (var u in users)
            {
                if (u.Id is not null && userRoleMap.TryGetValue(u.Id, out var rn))
                    u.Role = rn;
            }
        }
    }
}
