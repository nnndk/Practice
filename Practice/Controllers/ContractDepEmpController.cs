using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Practice.Controllers
{
    [Authorize(Roles = "Сотрудник договорного отдела")]
    public class ContractDepEmpController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
