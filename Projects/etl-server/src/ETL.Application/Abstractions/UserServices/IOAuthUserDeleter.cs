namespace ETL.Application.Abstractions.UserServices;
public interface IOAuthUserDeleter
{
    Task DeleteUserAsync(string userId, CancellationToken ct = default);
}
