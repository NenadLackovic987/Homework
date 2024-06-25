using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Homework.AspNetCoreMvc.Models;
using Homework.Common.Dto.OAuth;
using Homework.Common.Web.Network.Http;
using System.Security.Claims;

namespace Homework.AspNetCoreMvc.Controllers
{
    public class LoginController : BaseController
    {
        private readonly IConfiguration _configuration;

        public LoginController(IConfiguration configuration) : base(configuration)
        {
            _configuration = configuration;
        }

        [HttpGet]
        public IActionResult Index()
        {
            return !HttpContext.User.Identity.IsAuthenticated ? View() : RedirectToAction("Index", "Home"); 
        }

        [HttpGet]
        public IActionResult VerifyLogin()
        {
            return View("Index");
        }

        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Index", "Home");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> VerifyLogin(LoginViewModel model, CancellationToken cancellationToken)
        {
            if (ModelState.IsValid)
            {
                var httpBuilder = new HttpRequestBuilder(_configuration);
                await httpBuilder.MakeOAuthHandshakeAsync(new OAuthRequestDto() { Email = model.Email, Password = model.Password }, cancellationToken);

                if (httpBuilder.IsValidHandshake)
                {
                    AddCookie(httpBuilder.JwtToken, "jwt");
                    AddCookie(httpBuilder.JwtRefreshToken, "jwtRefreshCookie");
                    AddCookie(model.Email, "email");

                    var jwtMail = GetEmailFromJwt(httpBuilder.JwtToken);

                    var claims = new List<Claim>
                    {
                        new Claim(ClaimTypes.Name, jwtMail),
                        new Claim("FullName", jwtMail)
                    };

                    var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                    
                    SetDefaultLanguageCookieIfNotExists();

                    if (httpBuilder.NeedPasswordReset)
                    {
                        return RedirectToAction("ResetPassword", "Users");
                    }

                    await HttpContext.SignInAsync(
                        CookieAuthenticationDefaults.AuthenticationScheme,
                        new ClaimsPrincipal(claimsIdentity),
                        new AuthenticationProperties());

                    return RedirectToAction("Index", "Home");
                }

                if (!string.IsNullOrWhiteSpace(httpBuilder.ErrorMessage))
                {
                    ModelState.AddModelError(nameof(model.Email), httpBuilder.ErrorMessage);
                }
                else
                {
                    ModelState.AddModelError(nameof(model.Email), "Invalid login attempt, please check user params.");
                }

                return View("Index");
            }

            return View("Index",model);
        }

        private void AddCookie(string token, string name)
        {
            // Add new jwt
            var cookieOptions = new CookieOptions
            {
                HttpOnly = name != "lang", // Set HttpOnly to true
                Expires = DateTime.UtcNow.AddHours(8), // Set the cookie expiration time
                Secure = true // Goes over ssl
            };

            HttpContext.Response.Cookies.Append(name, token, cookieOptions);
        }

        private void SetDefaultLanguageCookieIfNotExists()
        {
            var cookie = HttpContext.Request.Cookies["lang"];

            if (string.IsNullOrWhiteSpace(cookie)) 
            {
                AddCookie("sr", "lang");
            }
        }
    }
}
