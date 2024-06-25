using Microsoft.AspNetCore.Mvc;
using Homework.AspNetCoreMvc.DataTransferObjects.User;
using Homework.AspNetCoreMvc.Models;
using Homework.Common;
using Homework.Common.Dto;

namespace Homework.AspNetCoreMvc.Controllers
{
    public class RegisterController : BaseController
    {
        private readonly IConfiguration _configuration;

        public RegisterController(IConfiguration configuration) : base(configuration)
        {
            _configuration = configuration;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> VerifyRegister(RegisterViewModel model, CancellationToken cancellationToken)
        {
            if (ModelState.IsValid)
            {
                var user = new RegisterRequestModel() { FirstName = model.FirstName, LastName = model.LasttName, Password = model.Password, Email = model.Email, Position = "test", NickName = "test", NeedPasswordReset = false};

                var result = await HttpBuilder.HttpPostAsync<RegisterRequestModel, BaseClientResult<UserResponse>, UserResponse>(Constants.API_POST_REGISTER_USER_PATH, Jwt, JwtRefreshToken, Language, user);

                if (result.IsValid)
                    return RedirectToAction("Index", "Login");
                else 
                    return AdaptWithHttpStatusCode(result);
            }
            return View("Index", model);
        }
    }
}
