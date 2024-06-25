using Mapster;
using Microsoft.AspNetCore.Identity;
using Homework.Application.Commands.Users;
using Homework.Application.Queries.Tokens;
using Homework.Application.Queries.Users;
using Homework.Common.Dto;
using Homework.Domain.Models.Identity;
using Homework.Web.Api.DataTransferObjects;

namespace Homework.Web.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : BaseController
    {
        private readonly IMediator _mediator;
        private IPasswordHasher<ApplicationUser> _passwordHasher;

        public UsersController(IMediator mediator, IPasswordHasher<ApplicationUser> passwordHash)
        {
            _mediator = mediator;
            _passwordHasher = passwordHash;
        }

        [HttpPost("Login")]
        public async Task<IActionResult> Login([FromBody] LoginApiRequest request)
        {
            var handlerResult = await _mediator.Send(request.Adapt<LoginQuery>());

            try
            {
                var apiResult = handlerResult.Adapt<BaseResult<LoginApiResponse>>();

                return AdaptWithHttpStatusCode(apiResult);
            }
            catch (Exception ex)
            {

                throw;
            }
        }

        [HttpPost("Register")]
        public async Task<IActionResult> Register([FromBody] RegisterUserApiRequest request)
        {
            var handlerResult = await _mediator.Send(request.Adapt<RegisterCommand>());

            var apiResult = handlerResult.Adapt<BaseResult<RegisterUserApiResponse>>();

            return AdaptWithHttpStatusCode(apiResult);
        }

        [HttpPost("RefreshToken")]
        public async Task<IActionResult> RefreshToken([FromBody] RefreshTokenApiRequest request)
        {
            var handlerResult = await _mediator.Send(request.Adapt<GetJwtRefreshTokenQuery>());

            var apiResult = handlerResult.Adapt<BaseResult<RefreshTokenApiResponse>>();

            return AdaptWithHttpStatusCode(apiResult);
        }
    }
}
