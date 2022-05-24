using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Practice.Controllers
{
    [Authorize(Roles = "Администратор баз данных")]
    public class DbAdminController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
