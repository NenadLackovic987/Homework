using Homework.Common.Dto;

namespace Homework.Web.Api.Controllers
{
    public class BaseController : ControllerBase
    {
        protected IActionResult AdaptWithHttpStatusCode<TData>(BaseResult<TData> result)
            => result.IsValid ? Ok(result) : Ok(result);
    }
}
