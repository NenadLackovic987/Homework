using Microsoft.AspNetCore.Mvc;
using Homework.Common.Dto;
using Homework.Common.Web.Network.Http;
using System.IdentityModel.Tokens.Jwt;

namespace Homework.AspNetCoreMvc.Controllers
{
    public class BaseController : Controller
    {
        protected readonly HttpRequestBuilder HttpBuilder;
        private readonly IConfiguration _configuration;
        public BaseController(IConfiguration configuration)
        {
            _configuration = configuration;
            HttpBuilder = new HttpRequestBuilder(configuration);
        }

        protected string Jwt { get { return Request.Cookies["jwt"]; } }

        protected string JwtRefreshToken { get { return Request.Cookies["jwtRefreshCookie"]; } }

        protected string Language { get { return Request.Cookies["lang"] ?? "sr"; } }

        protected IActionResult AdaptWithHttpStatusCode<TData>(BaseClientResult<TData> result)
            => result.IsValid ? Ok(result) : BadRequest();

        public string GetEmailFromJwt(string jwtToken)
        {
            var tokenHandler = new JwtSecurityTokenHandler();

            // Read the JWT token
            var token = tokenHandler.ReadJwtToken(jwtToken);

            // Access the email claim
            var emailClaim = token.Claims.FirstOrDefault(claim => claim.Type == "email");

            if (emailClaim != null)
            {
                return emailClaim.Value;
            }

            // Email claim not found in the token
            return null;
        }
    }
}
