using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Homework.AspNetCoreMvc.DataTransferObjects.User;
using Homework.AspNetCoreMvc.Models;
using Homework.Common;
using Homework.Common.Dto;

namespace Homework.AspNetCoreMvc.Controllers
{
    [Authorize]
    public class UsersController : BaseController
    {
        private readonly IWebHostEnvironment _webHostEnvironment;
        public UsersController(IConfiguration configuration, IWebHostEnvironment webHostEnvironment) 
            : base(configuration)
        {
            _webHostEnvironment = webHostEnvironment;
        }

        [HttpGet]
        [Route("Login/ConfirmResetPassword")]
        [AllowAnonymous]
        public async Task<IActionResult> ConfirmResetPassword([FromQuery] Guid sessionId)
        {
            var model = new ConfirmViewModel() { SessionId = sessionId };
            var result = await HttpBuilder.HttpPutAsync<ConfirmViewModel, BaseClientResult<ConfirmResetPasswordResponse>, ConfirmResetPasswordResponse>(Constants.API_CONFIRM_USER_PASSWORD_RESET, Jwt, JwtRefreshToken, Language, model);
            return RedirectToAction("Index", "Login");
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult ResetPassword()
        {
            return View();
        }


        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> RequireResetPassword(ResetPasswordViewModel model)
        {
            var result = await HttpBuilder.HttpPostAsync<ResetPasswordViewModel, BaseClientResult<CreateResetSessionResponse>, CreateResetSessionResponse>(Constants.API_CREATE_RESET_SESSION, Jwt, JwtRefreshToken, Language, model);
            return RedirectToAction("ResetInfo");
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult ResetInfo()
        {
            return View();
        }
    }
}
