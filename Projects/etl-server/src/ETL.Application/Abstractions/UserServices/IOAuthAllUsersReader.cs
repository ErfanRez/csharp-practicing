using ETL.Application.Common.DTOs;

namespace ETL.Application.Abstractions.UserServices
{
    public interface IOAuthAllUserReader
    {
        Task<List<UserDto>> GetAllAsync(int? first = null, int? max = null, CancellationToken ct = default);
    }
}
