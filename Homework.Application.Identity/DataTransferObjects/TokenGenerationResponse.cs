namespace Homework.Application.Identity.DataTransferObjects
{
    public class TokenGenerationResponse
    {
        public string? JwtToken { get; set; }
        public string? JwtRefreshToken { get; set; }
    }
}
