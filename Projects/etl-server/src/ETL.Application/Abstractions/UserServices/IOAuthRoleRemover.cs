namespace ETL.Application.Abstractions.UserServices;
public interface IOAuthRoleRemover
{
    public Task RemoveAllRealmRolesAsync(string userId, CancellationToken ct = default);
}
