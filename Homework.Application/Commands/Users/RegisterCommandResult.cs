namespace Homework.Application.Commands.Users
{
    public class RegisterCommandResult
    {
        public string? JwtToken { get; set; }
        public string? JwtRefreshToken { get; set; }
    }
}

