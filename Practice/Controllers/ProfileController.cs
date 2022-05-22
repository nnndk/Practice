using Microsoft.AspNetCore.Mvc;

namespace Practice.Controllers
{
    public class ProfileController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
