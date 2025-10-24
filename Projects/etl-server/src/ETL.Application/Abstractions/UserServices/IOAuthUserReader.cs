using ETL.Application.Common.DTOs;

namespace ETL.Application.Abstractions.UserServices;

public interface IOAuthUserReader
{
    Task<UserDto> GetByIdAsync(string userId, CancellationToken ct = default);
}
