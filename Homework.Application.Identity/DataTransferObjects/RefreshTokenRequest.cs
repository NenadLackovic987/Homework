namespace Homework.Application.Identity.DataTransferObjects
{
    public class RefreshTokenRequest
    {
        public string? JwtRefreshToken { get; set; }
        public TimeSpan TokenLifeTime { get; set; }
        public TimeSpan RefreshTokenLifeTime { get; set; }
    }
}
