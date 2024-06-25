namespace Homework.Common.Web.Contracts.Network.Http
{
    public interface IOAuthResult
    {
        public string? JwtToken { get; set; }
        public string? JwtRefreshToken { get; set; }
    }
}
