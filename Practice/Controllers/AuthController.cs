using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Practice.Data;
using Practice.Helper;
using Practice.Models;
using Practice.Models.ViewModels;
using System.Security.Claims;

namespace Practice.Controllers
{
    public class AuthController : Controller
    {
        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (ModelState.IsValid)
            {
                using (var db = new CourseProject2DBContext())
                {
                    Сотрудники? employee = (from emp in db.Сотрудникиs
                                            where emp.Логин == model.Login
                                            select emp).FirstOrDefault();

                    if (employee == null || !Auth.VerifyHashedPassword(employee.Пароль, model.Password))
                    {
                        ModelState.AddModelError("Login", "Неправильный логин и/или пароль!");
                        ModelState.AddModelError("Password", "Неправильный логин и/или пароль!");
                        return View(model);
                    }

                    // создаем один claim
                    var claims = new List<Claim> { new Claim(ClaimsIdentity.DefaultNameClaimType, model.Login) };

                    // добавляем роли
                    foreach (var role in Queries.GetEmployeeRoles(model.Login))
                        claims.Add(new Claim(ClaimTypes.Role, role.Key));

                    // создаем объект ClaimsIdentity
                    ClaimsIdentity claimsIdentity = new ClaimsIdentity(claims, "ApplicationCookie", ClaimsIdentity.DefaultNameClaimType, ClaimsIdentity.DefaultRoleClaimType);
                    // установка аутентификационных куки
                    await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(claimsIdentity));

                    return LocalRedirect("/");
                }
            }

            return View(model);
        }

        [Authorize]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Login");
        }
    }
}
