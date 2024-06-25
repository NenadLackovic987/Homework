using Mapster;
using Microsoft.AspNetCore.Authorization;
using Homework.Application.Commands.Users;
using Homework.Common.Dto;
using Homework.Web.Api.DataTransferObjects.Users;

namespace Homework.Web.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [AllowAnonymous]
    public class AdministrationController : BaseController
    {
        private readonly IMediator _mediator;

        public AdministrationController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPut("ConsumeUserSession")]
        public async Task<IActionResult> ConsumeUserSession([FromBody] ConfirmUserSessionApiRequest request)
        {
            var handlerResult = await _mediator.Send(request.Adapt<ConsumeUserSessionCommand>());

            var result = new BaseResult<ConfirmUserSessionApiResponse>() { ErrorCodes = handlerResult.ErrorCodes, Data =  new ConfirmUserSessionApiResponse() { Email = handlerResult?.Data?.Email} };

            return AdaptWithHttpStatusCode(result);
        }

        [HttpPost("CreateResetSession")]
        public async Task<IActionResult> CreateResetSession([FromBody] CreateResetSessionApiRequest request)
        {
            var handlerResult = await _mediator.Send(request.Adapt<CreateResetSessionCommand>());

            var result = new BaseResult<CreateResetSessionApiResponse>() { ErrorCodes = handlerResult.ErrorCodes };

            return AdaptWithHttpStatusCode(result);
        }        
    }
}
