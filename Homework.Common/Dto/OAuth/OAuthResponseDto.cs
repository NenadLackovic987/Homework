namespace Homework.Common.Dto.OAuth
{
    public class OAuthResponseDto
    {
        public string JwtToken { get; set; } = null!;
        public string JwtRefreshToken { get; set; } = null!;
        public bool NeedPasswordReset { get; set; }
    }
}
