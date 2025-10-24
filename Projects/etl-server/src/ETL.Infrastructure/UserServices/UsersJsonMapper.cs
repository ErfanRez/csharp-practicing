using System.Text.Json;
using ETL.Application.Common.DTOs;
using ETL.Infrastructure.UserServices.Abstractions;

namespace ETL.Infrastructure.UserServices
{
    public class UserJsonMapper : IUserJsonMapper
    {
        public UserDto Map(JsonElement userJson)
        {
            return new UserDto
            {
                Id = userJson.TryGetProperty("id", out var pId) ? pId.GetString() : null,
                Username = userJson.TryGetProperty("username", out var pUsername) ? pUsername.GetString() : null,
                Email = userJson.TryGetProperty("email", out var pEmail) ? pEmail.GetString() : null,
                FirstName = userJson.TryGetProperty("firstName", out var pFirst) ? pFirst.GetString() : null,
                LastName = userJson.TryGetProperty("lastName", out var pLast) ? pLast.GetString() : null,
                Role = null
            };
        }
    }
}
