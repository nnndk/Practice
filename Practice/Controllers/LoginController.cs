using Microsoft.AspNetCore.Mvc;

namespace Practice.Controllers
{
    public class LoginController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
