using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Practice.Data;
using Practice.Models;
using Practice.Helper;

namespace Practice.Controllers
{
    [Authorize(Roles = "Администратор")]
    public class AdminController : Controller
    {
        public IActionResult Index()
        {
            using (var db = new CourseProject2DBContext())
            {
                var emps = new List<Dictionary<string, string>>();

                var query = from emp in db.Сотрудникиs
                            join sex in db.Полs on emp.КодПола equals sex.Код
                            join empType in db.ВидыТрудоустройстваs on emp.КодВидаТрудоустройства equals empType.Код
                            where (
                            from employee in db.УстройствоНаРаботуs
                            where employee.ДатаУвольнения > DateTime.Now.Date || employee.ДатаУвольнения == null
                            select employee.КодСотрудника).ToList().Contains(emp.Код)
                            select new
                            {
                                Код = emp.Код,
                                Фамилия = emp.Фамилия,
                                Имя = emp.Имя,
                                Отчество = emp.Отчество,
                                Пол = sex.Пол1,
                                ДатаРождения = emp.ДатаРождения.Date.ToString(),
                                Телефон1 = emp.Телефон1,
                                Телефон2 = emp.Телефон2,
                                ДатаНачалаРаботыВSap = emp.ДатаНачалаРаботыВSap.Value.Date.ToString(),
                                ВидТрудоустройства = empType.ВидТрудоустройства,
                                Логин = emp.Логин
                            };

                int x = query.Count();

                foreach (var item in query)
                {
                    var json_emps = JsonConvert.SerializeObject(item);
                    emps.Add(JsonConvert.DeserializeObject<Dictionary<string, string>>(json_emps));
                }

                if (emps.Count == 0)
                {
                    var list = new List<Dictionary<string, string>>();
                    var dict = new Dictionary<string, string>();
                    var keys = new string[] { "Код", "Фамилия", "Имя", "Отчество", "Пол", "ДатаРождения", "Телефон1", "Телефон2", "ДатаНачалаРаботыВSap", "ВидТрудоустройства", "Логин" };

                    foreach (var key in keys)
                        dict.Add(key, null);

                    emps.Add(dict);
                }

                return View(emps);
            }
        }

        [HttpGet]
        public IActionResult AddEmployee()
        {
            return View(new Сотрудники());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult AddEmployee(Сотрудники emp)
        {
            using (var db = new CourseProject2DBContext())
            {
                if (ModelState.ErrorCount == 2)
                {
                    emp.Пароль = Auth.HashPassword(emp.Пароль);

                    db.Сотрудникиs.Add(emp);
                    db.SaveChanges();

                    return RedirectToAction("Index");
                }
            }

            return View(emp);
        }

        [HttpGet]
        public IActionResult UpdateEmployee(long id = -1)
        {
            if (id == -1)
                NotFound();

            using (var db = new CourseProject2DBContext())
            {
                var newEmp = (from emp in db.Сотрудникиs
                              join sex in db.Полs on emp.КодПола equals sex.Код
                              join empType in db.ВидыТрудоустройстваs on emp.КодВидаТрудоустройства equals empType.Код
                              where (
                              from employee in db.УстройствоНаРаботуs
                              where employee.ДатаУвольнения > DateTime.Now.Date || employee.ДатаУвольнения == null
                              select employee.КодСотрудника).ToList().Contains(emp.Код) && emp.Код == id
                              select emp).First();

                if (newEmp == null)
                    return NotFound();

                newEmp.Пароль = "";

                return View(newEmp);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult UpdateEmployee(Сотрудники emp)
        {
            if (emp.Пароль != null && emp.Пароль.Length < 8)
            {
                ModelState.AddModelError("Пароль", "Ошибка! Минимальная длина пароля - 8 символов!");
                return View(emp);
            }

            using (var db = new CourseProject2DBContext())
            {
                if (ModelState.ErrorCount <= 3)
                {
                    if (emp.Пароль == null)
                    {
                        emp.Пароль = (from employee in db.Сотрудникиs
                                      where employee.Код == emp.Код
                                      select employee.Пароль).First();
                    }
                    else
                        emp.Пароль = Auth.HashPassword(emp.Пароль);

                    db.Сотрудникиs.Update(emp);
                    db.SaveChanges();

                    return RedirectToAction("Index");
                }
            }

            return View(emp);
        }
    }
}
