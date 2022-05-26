using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Practice.Data;
using Practice.Models;
using System.Diagnostics;
using Practice.Helper;

namespace Practice.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        private readonly Dictionary<string, string> dictRolesPosViews = new Dictionary<string, string>() {
            { "Разработчик", "Developer" },
            { "Менеджер проекта", "ProjectManager" },
            { "Администратор", "Admin" },
            { "Администратор баз данных", "DbAdmin" },
            { "Директор департамента", "DepDirector" },
            { "Генеральный директор", "CEO" },
            { "Бухгалтер", "Accountant" },
            { "Сотрудник договорного отдела", "ContractDepEmp" }
        };

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            if (HttpContext.User.Identity.Name != null)
                return View(Queries.GetEmployeeRoles(User.Identity.Name));

            return View(new Dictionary<string, string>());
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}