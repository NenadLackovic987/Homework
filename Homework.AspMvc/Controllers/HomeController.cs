using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Homework.AspNetCoreMvc.Controllers
{
    [Authorize]
    public class HomeController : BaseController
    {
        public HomeController(IConfiguration configuration) : base(configuration)
        {
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult About()
        {
            return View();
        }

        public IActionResult Error()
        {
            return View();
        }
    }
}
