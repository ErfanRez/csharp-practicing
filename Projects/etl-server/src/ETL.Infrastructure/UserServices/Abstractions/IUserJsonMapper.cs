using System.Text.Json;
using ETL.Application.Common.DTOs;

namespace ETL.Infrastructure.UserServices.Abstractions
{
    public interface IUserJsonMapper
    {
        UserDto Map(JsonElement userJson);
    }
}

