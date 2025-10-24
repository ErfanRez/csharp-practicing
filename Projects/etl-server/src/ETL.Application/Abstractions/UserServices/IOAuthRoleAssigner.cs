namespace ETL.Application.Abstractions.UserServices;

public interface IOAuthRoleAssigner
{
    Task AssignRoleAsync(string userId, string? roleName, CancellationToken ct = default);
}
