namespace ETL.Application.Common.DTOs
{
    public class UserDto
    {
        public string? Id { get; set; }
        public required string Username { get; set; }
        public required string Email { get; set; }
        public required string FirstName { get; set; }
        public required string LastName { get; set; }
        public string? Role { get; set; }
    }
}