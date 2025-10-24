namespace ETL.Application.Abstractions.UserServices;

public interface IOAuthUserRoleGetter
{
    Task<string?> GetRoleForUserAsync(string userId, CancellationToken ct = default);
}
