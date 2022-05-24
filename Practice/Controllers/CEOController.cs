using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Practice.Controllers
{
    [Authorize(Roles = "Генеральный директор")]
    public class CEOController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
