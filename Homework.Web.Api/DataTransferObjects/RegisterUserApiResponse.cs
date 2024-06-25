namespace Homework.Web.Api.DataTransferObjects
{
    public class RegisterUserApiResponse
    {
        public string? JwtToken { get; set; }
        public string? JwtRefreshToken { get; set; }
    }
}
