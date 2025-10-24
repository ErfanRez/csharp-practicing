using ETL.Application.User.Edit;

namespace ETL.Application.Abstractions.UserServices;

public interface IOAuthUserUpdater
{
    Task UpdateUserAsync(EditUserCommand command, CancellationToken ct = default);
}
