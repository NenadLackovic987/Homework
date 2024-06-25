namespace Homework.Application.Queries.Users
{
    public class LoginQueryResult
    {
        public string? JwtToken { get; set; }
        public string? JwtRefreshToken { get; set; }
        public bool NeedPasswordReset { get; set; }
    }
}
