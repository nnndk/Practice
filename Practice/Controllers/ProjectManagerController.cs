using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Practice.Controllers
{
    [Authorize(Roles = "Менеджер проекта")]
    public class ProjectManagerController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
