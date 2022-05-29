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
                              select emp).FirstOrDefault();

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

        public IActionResult EmployeePositions(long id = -1)
        {
            if (id == -1)
                return NotFound();

            using (var db = new CourseProject2DBContext())
            {
                var positions = new List<Dictionary<string, string>>();

                var query = from empPos in db.ДолжностиСотрудниковs
                            join dep in db.Департаментыs on empPos.КодДепартамента equals dep.Код
                            join pos in db.Должностиs on empPos.КодДолжности equals pos.Код
                            where empPos.КодСотрудника == id
                            select new
                            {
                                Код = empPos.Код,
                                НазваниеДепартамента = dep.НазваниеДепартамента,
                                ДатаНазначения = empPos.ДатаНазначения.ToString("yyyy-MM-dd HH:mm:ss"),
                                Должность = pos.Должность
                            };

                foreach (var item in query)
                {
                    var json_emps = JsonConvert.SerializeObject(item);
                    positions.Add(JsonConvert.DeserializeObject<Dictionary<string, string>>(json_emps));
                }

                if (positions.Count == 0)
                {
                    var list = new List<Dictionary<string, string>>();
                    var dict = new Dictionary<string, string>();
                    var keys = new string[] { "НазваниеДепартамента", "ДатаНазначения", "Должность" };

                    foreach (var key in keys)
                        dict.Add(key, null);

                    positions.Add(dict);
                }

                ViewBag.EmpId = id;

                return View(positions);
            }
        }

        [HttpGet]
        public IActionResult DeletePosition(long id = -1)
        {
            if (id == -1)
                return NotFound();

            using (var db = new CourseProject2DBContext())
            {
                var query = (from emp_pos in db.ДолжностиСотрудниковs
                            join dep in db.Департаментыs on emp_pos.КодДепартамента equals dep.Код
                            join pos in db.Должностиs on emp_pos.КодДолжности equals pos.Код
                            where emp_pos.Код == id
                            select new
                            {
                                Код = emp_pos.Код,
                                НазваниеДепартамента = dep.НазваниеДепартамента,
                                КодСотрудника = emp_pos.КодСотрудника,
                                ДатаНазначения = emp_pos.ДатаНазначения,
                                Должность = pos.Должность
                            }).FirstOrDefault();

                if (query == null)
                    return NotFound();

                var json_pos= JsonConvert.SerializeObject(query);
                var dict_pos = JsonConvert.DeserializeObject<Dictionary<string, string>>(json_pos);

                if (query.Должность == "Директор департамента")
                    return NotFound();

                return View(dict_pos);
            }
        }

        [HttpPost, ActionName("DeletePosition")]
        [ValidateAntiForgeryToken]
        public IActionResult DeletePositionPOST(long id = -1)
        {
            using (var db = new CourseProject2DBContext())
            {
                var pos = db.ДолжностиСотрудниковs.Find(id);

                if (pos == null)
                    return NotFound();

                db.ДолжностиСотрудниковs.Remove(pos);
                db.SaveChanges();
                TempData["success"] = "Employee position deleted successfully";

                return RedirectToAction("EmployeePositions", new { id = pos.КодСотрудника });
            }
        }

        [HttpGet]
        public IActionResult AddPosition(long id = -1)
        {
            if (id == -1)
                return NotFound();

            return View(new ДолжностиСотрудников() { КодСотрудника = id });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult AddPosition(ДолжностиСотрудников empPosition)
        {
            using (var db = new CourseProject2DBContext())
            {
                var query = (from empPos in db.ДолжностиСотрудниковs
                             where empPos.КодДепартамента == empPosition.КодДепартамента
                             && empPos.КодСотрудника == empPosition.КодСотрудника
                             && empPos.КодДолжности == empPosition.КодДолжности
                             select empPos).Any();

                if (query)
                {
                    ModelState.AddModelError("КодДолжности", "Ошибка! В этом департаменте указанный сотрудник уже имеет такую должность!");

                    return View(empPosition);
                }

                if (ModelState.ErrorCount == 3)
                {
                    db.ДолжностиСотрудниковs.Add(empPosition);
                    db.SaveChanges();

                    return RedirectToAction("EmployeePositions", new { id = empPosition.КодСотрудника });
                }
            }

            return View(empPosition);
        }
    }
}
