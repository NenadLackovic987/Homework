using Homework.Common.Web.Contracts.Network.Http;

namespace Homework.Web.Api.DataTransferObjects
{
    public class LoginApiResponse : IOAuthResult
    {
        public string? JwtToken { get; set; }
        public string? JwtRefreshToken { get; set; }
        public bool NeedPasswordReset { get; set; }
    }
}
