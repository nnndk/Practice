using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Practice.Controllers
{
    [Authorize(Roles = "Директор департамента")]
    public class DepDirectorController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
