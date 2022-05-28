using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Practice.Data;
using Practice.Helper;
using Practice.Models;
using System.Security.Claims;

namespace Practice.Controllers
{
    public class LoginController : Controller
    {
        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }

        /*public async Task<IActionResult> Index(string? returnUrl, HttpContext context)
        {
            // получаем из формы email и пароль
            var form = context.Request.Form;
            // если email и/или пароль не установлены, посылаем статусный код ошибки 400
            if (form["login"] == "" || form["password"] == "")
            {
                ViewBag.Message = "Логин и/или пароль не установлены";
                return View();
            }

            string login = form["login"];
            string password = form["password"];
            Сотрудники? employee;

            using (var db = new CourseProject2DBContext())
            {
                employee = (from emp in db.Сотрудникиs
                            where emp.Логин == login
                            select emp).FirstOrDefault();
            }

            if (employee is null || !Auth.VerifyHashedPassword(employee.Пароль, password))
                //return Results.Unauthorized(); // code 401
                return Results.LocalRedirect("~/Login/Index/1");

            var claims = new List<Claim> { new Claim(ClaimTypes.Name, employee.Логин) };

            foreach (var role in Queries.GetEmployeeRoles(login))
                claims.Add(new Claim(ClaimTypes.Role, role.Key));

            // создаем объект ClaimsIdentity
            ClaimsIdentity claimsIdentity = new ClaimsIdentity(claims, "Cookies");
            // установка аутентификационных куки
            await context.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(claimsIdentity));

            return Results.Redirect(returnUrl ?? "/");
        }*/

        [Authorize]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Index", "Login");
        }
    }
}
