using ETL.Application.User.Create;

namespace ETL.Application.Abstractions.UserServices;

public interface IOAuthUserCreator
{
    Task<string> CreateUserAsync(CreateUserCommand command, CancellationToken ct = default);
}
