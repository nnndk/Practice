using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Practice.Data;
using Practice.Models;
using Practice.Models.ViewModels;

namespace Practice.Controllers
{
    [Authorize(Roles = "Сотрудник договорного отдела")]
    public class ContractDepEmpController : Controller
    {
        public IActionResult Index()
        {
            using (var db = new CourseProject2DBContext())
            {
                var projects = new List<Dictionary<string, string>>();
                var query1 = from project in db.Проектыs
                             join projectType in db.ТипыПроектовs on project.КодТипаПроекта equals projectType.Код
                             select new
                             {
                                 Код = project.Код,
                                 НазваниеПроекта = project.НазваниеПроекта,
                                 ДатаНачалаПроекта = project.ДатаНачалаПроекта.Value.Date.ToString("yyyy-MM-dd HH:mm:ss"),
                                 ДатаЗавершенияПроекта = project.ДатаЗавершенияПроекта.Value.Date.ToString("yyyy-MM-dd HH:mm:ss"),
                                 КодМенеджераПроекта = project.КодМенеджераПроекта,
                                 ТипПроекта = projectType.ТипПроекта
                             };

                foreach (var project in query1)
                {
                    var json_pr = JsonConvert.SerializeObject(project);
                    projects.Add(JsonConvert.DeserializeObject<Dictionary<string, string>>(json_pr));
                }

                if (projects.Count == 0)
                {
                    var keys = new string[] { "Код", "НазваниеПроекта", "ДатаНачалаПроекта", "ДатаЗавершенияПроекта", "КодМенеджераПроекта", "ТипПроекта" };
                    var dict = new Dictionary<string, string>();

                    foreach (var key in keys)
                        dict.Add(key, null);

                    projects.Add(dict);
                }

                var emps = new List<Dictionary<string, string>>();
                var query2 = from emp in db.Сотрудникиs
                             join sex in db.Полs on emp.КодПола equals sex.Код
                             join empType in db.ВидыТрудоустройстваs on emp.КодВидаТрудоустройства equals empType.Код
                             join employment in db.УстройствоНаРаботуs on emp.Код equals employment.КодСотрудника
                             where employment.ДатаУвольнения > DateTime.Now.Date || employment.ДатаУвольнения == null
                             select new
                             {
                                 Код = emp.Код,
                                 Фамилия = emp.Фамилия,
                                 Имя = emp.Имя,
                                 Отчество = emp.Отчество,
                                 Пол = sex.Пол1,
                                 ДатаРождения = emp.ДатаРождения.Value.Date.ToString("yyyy-MM-dd HH:mm:ss"),
                                 Телефон1 = emp.Телефон1,
                                 Телефон2 = emp.Телефон2,
                                 ДатаНачалаРаботыВSap = emp.ДатаНачалаРаботыВSap.Value.Date.ToString("yyyy-MM-dd HH:mm:ss"),
                                 ВидТрудоустройства = empType.ВидТрудоустройства,
                                 Логин = emp.Логин,
                                 ДатаЗачисленияВШтат = employment.ДатаЗачисленияВШтат.Value.Date.ToString("yyyy-MM-dd HH:mm:ss"),
                                 ДатаУвольнения = employment.ДатаУвольнения.Value.Date.ToString("yyyy-MM-dd HH:mm:ss")
                             };

                foreach (var emp in query2)
                {
                    var json_emp = JsonConvert.SerializeObject(emp);
                    emps.Add(JsonConvert.DeserializeObject<Dictionary<string, string>>(json_emp));
                }

                if (emps.Count == 0)
                {
                    var keys = new string[] { "Код", "Фамилия", "Имя", "Отчество", "Пол", "ДатаРождения", "Телефон1", "Телефон2", "ДатаНачалаРаботыВSap",
                        "ВидТрудоустройства", "Логин", "ДатаЗачисленияВШтат", "ДатаУвольнения" };
                    var dict = new Dictionary<string, string>();

                    foreach (var key in keys)
                        dict.Add(key, null);

                    emps.Add(dict);
                }

                var deps = (from dep in db.Департаментыs
                            select dep).ToList();

                return View(new ContractDepEmpViewModel() { projects = projects, employees = emps, departments = deps });
            }
        }

        [HttpGet]
        public IActionResult AddProject()
        {
            return View(new Проекты());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult AddProject(Проекты project)
        {
            using (var db = new CourseProject2DBContext())
            {
                if (ModelState.ErrorCount == 3)
                {
                    db.Проектыs.Add(project);
                    db.SaveChanges();

                    return RedirectToAction("Index");
                }
            }

            return View(project);
        }

        [HttpGet]
        public IActionResult UpdateProject(int id = -1)
        {
            if (id == -1)
                return NotFound();

            using (var db = new CourseProject2DBContext())
            {
                var project = (from pr in db.Проектыs
                               where pr.Код == id
                               select pr).FirstOrDefault();

                if (project == null)
                    return NotFound();

                return View(project);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult UpdateProject(Проекты project)
        {
            if (project.ДатаЗавершенияПроекта != null && project.ДатаНачалаПроекта > project.ДатаЗавершенияПроекта)
            {
                ModelState.AddModelError("ДатаЗавершенияПроекта", "Ошибка! Дата завершения проекта не может быть меньше, чем дата его начала!");
                return View(project);
            }

            using (var db = new CourseProject2DBContext())
            {
                if (ModelState.ErrorCount == 2)
                {
                    db.Проектыs.Update(project);
                    db.SaveChanges();

                    return RedirectToAction("Index");
                }
            }

            return View(project);
        }

        public IActionResult GetUnemployedPersons()
        {
            using (var db = new CourseProject2DBContext())
            {
                var emps = new List<Dictionary<string, string>>();
                var query2 = from emp in db.Сотрудникиs
                             join sex in db.Полs on emp.КодПола equals sex.Код
                             join empType in db.ВидыТрудоустройстваs on emp.КодВидаТрудоустройства equals empType.Код
                             where !(from employment in db.УстройствоНаРаботуs
                                     where employment.ДатаУвольнения > DateTime.Now.Date || employment.ДатаУвольнения == null
                                     select employment.КодСотрудника).Distinct().ToList().Contains(emp.Код)
                             select new
                             {
                                 Код = emp.Код,
                                 Фамилия = emp.Фамилия,
                                 Имя = emp.Имя,
                                 Отчество = emp.Отчество,
                                 Пол = sex.Пол1,
                                 ДатаРождения = emp.ДатаРождения.Value.Date.ToString("yyyy-MM-dd HH:mm:ss"),
                                 Телефон1 = emp.Телефон1,
                                 Телефон2 = emp.Телефон2,
                                 ДатаНачалаРаботыВSap = emp.ДатаНачалаРаботыВSap.Value.Date.ToString("yyyy-MM-dd HH:mm:ss"),
                                 ВидТрудоустройства = empType.ВидТрудоустройства,
                                 Логин = emp.Логин
                             };

                foreach (var emp in query2)
                {
                    var json_emp = JsonConvert.SerializeObject(emp);
                    emps.Add(JsonConvert.DeserializeObject<Dictionary<string, string>>(json_emp));
                }

                if (emps.Count == 0)
                {
                    var keys = new string[] { "Код", "Фамилия", "Имя", "Отчество", "Пол", "ДатаРождения", "Телефон1", "Телефон2", "ДатаНачалаРаботыВSap",
                        "ВидТрудоустройства", "Логин" };
                    var dict = new Dictionary<string, string>();

                    foreach (var key in keys)
                        dict.Add(key, null);

                    emps.Add(dict);
                }

                return View(emps);
            }
        }

        [HttpGet]
        public IActionResult HireEmployee(int id = -1)
        {
            if (id == -1)
                return NotFound();

            using (var db = new CourseProject2DBContext())
            {
                // есть ли искомый сотрудник в базе (добавил ли его администратор) и не числится ли он в штате в данный момент
                var query = (from emp in db.Сотрудникиs
                             where !(from empl in db.УстройствоНаРаботуs
                                     where empl.ДатаУвольнения > DateTime.Now.Date || empl.ДатаУвольнения == null
                                     select empl.КодСотрудника).Distinct().ToList().Contains(id) && emp.Код == id
                             select emp.Код).Any();

                if (!query)
                    return NotFound();

                return View(new УстройствоНаРаботу() { КодСотрудника = id });
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult HireEmployee(УстройствоНаРаботу employment)
        {
            using (var db = new CourseProject2DBContext())
            {
                var query = (from empl in db.УстройствоНаРаботуs
                             where empl.КодСотрудника == employment.КодСотрудника
                             && (empl.ДатаЗачисленияВШтат >= employment.ДатаЗачисленияВШтат
                             || empl.ДатаУвольнения >= employment.ДатаЗачисленияВШтат)
                             select empl).Any();

                if (query)
                {
                    ModelState.AddModelError("ДатаЗачисленияВШтат", "Ошибка! Дата зачисления в штат должна быть больше, чем " +
                        "дата последнего увольнения!");
                }

                if (ModelState.ErrorCount == 1)
                {
                    db.УстройствоНаРаботуs.Add(employment);
                    db.SaveChanges();

                    return RedirectToAction("GetUnemployedPersons");
                }
            }

            return View(employment);
        }

        [HttpGet]
        public IActionResult FireEmployee(int id = -1)
        {
            if (id == -1)
                return NotFound();

            using (var db = new CourseProject2DBContext())
            {
                // есть ли искомый сотрудник в базе (добавил ли его администратор) и не числится ли он в штате в данный момент
                var empl = (from employment in db.УстройствоНаРаботуs
                            where employment.КодСотрудника == id && (employment.ДатаУвольнения > DateTime.Now.Date || employment.ДатаУвольнения == null)
                            select employment).FirstOrDefault();

                if (empl == null)
                    return NotFound();

                return View(empl);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult FireEmployee(УстройствоНаРаботу employment)
        {
            if (employment.ДатаУвольнения < employment.ДатаЗачисленияВШтат)
            {
                ModelState.AddModelError("ДатаУвольнения", "Ошибка! Дата увольнения не может быть меньше даты зачисления в штат!");
                return View(employment);
            }

            using (var db = new CourseProject2DBContext())
            {
                if (ModelState.ErrorCount == 1)
                {
                    db.УстройствоНаРаботуs.Update(employment);
                    db.SaveChanges();

                    return RedirectToAction("Index");
                }
            }

            return View(employment);
        }

        [HttpGet]
        public IActionResult GetAllTasks()
        {
            using (var db = new CourseProject2DBContext())
            {
                var tasks = new List<Dictionary<string, string>>();

                var query1 = from task in db.ФактическиеТрудозатратыs
                             join status in db.Статусыs on task.КодСтатуса equals status.Код
                             join emp in db.Сотрудникиs on task.КодРазработчика equals emp.Код
                             orderby status.Статус, task.ПоследнееИзменение descending
                             select new
                             {
                                 Код = task.Код,
                                 КодРазработчика = task.КодРазработчика,
                                 КодПроекта = task.КодПроекта,
                                 Задача = task.Задача,
                                 ДатаТрудозатраты = task.ДатаТрудозатраты.Value.ToString("yyyy-MM-dd HH:mm:ss"),
                                 КоличествоЧасов = task.КоличествоЧасов,
                                 Комментарий = task.Комментарий,
                                 Статус = status.Статус,
                                 ПоследнееИзменение = task.ПоследнееИзменение.ToString("yyyy-MM-dd HH:mm:ss")
                             };

                foreach (var item in query1)
                {
                    var json_tasks = JsonConvert.SerializeObject(item);
                    var dict_tasks = JsonConvert.DeserializeObject<Dictionary<string, string>>(json_tasks);

                    tasks.Add(dict_tasks);
                }

                if (tasks.Count == 0)
                {
                    var keys = new string[] { "Код", "КодРазработчика", "Логин", "КодПроекта", "Задача",
                        "ДатаТрудозатраты", "КоличествоЧасов", "Комментарий", "Статус", "ПоследнееИзменение" };
                    var dict = new Dictionary<string, string>();

                    foreach (var key in keys)
                        dict.Add(key, null);

                    tasks.Add(dict);
                }

                return View(tasks);
            }
        }

        [HttpGet]
        public IActionResult AddDepartment()
        {
            return View(new Департаменты());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult AddDepartment(Департаменты dep)
        {
            using (var db = new CourseProject2DBContext())
            {
                var query = (from emp in db.Сотрудникиs
                             where (from employee in db.УстройствоНаРаботуs
                                    where employee.ДатаУвольнения > DateTime.Now.Date || employee.ДатаУвольнения == null
                                    select employee.КодСотрудника).ToList().Contains(emp.Код) && emp.Код == dep.КодДиректораДепартамента
                             select emp.Код).Any();

                if (!query)
                {
                    ModelState.AddModelError("КодДиректораДепартамента", "Ошибка! Нет сотрудника с таким кодом!");
                    return View(dep);
                }

                if (ModelState.ErrorCount == 2)
                {
                    db.Департаментыs.Add(dep);
                    db.SaveChanges();

                    db.ДолжностиСотрудниковs.Add(new ДолжностиСотрудников()
                    {
                        КодДепартамента = dep.Код,
                        КодСотрудника = dep.КодДиректораДепартамента,
                        ДатаНазначения = DateTime.Now.Date,
                        КодДолжности = 8
                    });
                    db.SaveChanges();

                    return RedirectToAction("Index");
                }
            }

            return View(dep);
        }

        [HttpGet]
        public IActionResult UpdateDepartment(int id = -1)
        {
            if (id == -1)
                return NotFound();

            using (var db = new CourseProject2DBContext())
            {
                var department = (from dep in db.Департаментыs
                                  where dep.Код == id
                                  select dep).FirstOrDefault();

                return View(department);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult UpdateDepartment(Департаменты dep)
        {
            using (var db = new CourseProject2DBContext())
            {
                var query = (from emp in db.Сотрудникиs
                             where (from employee in db.УстройствоНаРаботуs
                                    where employee.ДатаУвольнения > DateTime.Now.Date || employee.ДатаУвольнения == null
                                    select employee.КодСотрудника).ToList().Contains(emp.Код) && emp.Код == dep.КодДиректораДепартамента
                             select emp.Код).Any();

                if (!query)
                {
                    ModelState.AddModelError("КодДиректораДепартамента", "Ошибка! Нет сотрудника с таким кодом!");
                    return View(dep);
                }

                long? oldDepDir = (from department in db.Департаментыs
                                   where department.Код == dep.Код
                                   select department.КодДиректораДепартамента).First();

                if (ModelState.ErrorCount == 1)
                {
                    db.Департаментыs.Update(dep);
                    db.SaveChanges();

                    if (oldDepDir != dep.КодДиректораДепартамента)
                    {
                        var pos = (from emp_pos in db.ДолжностиСотрудниковs
                                   where emp_pos.КодСотрудника == oldDepDir && emp_pos.КодДолжности == 8
                                   select emp_pos).FirstOrDefault();

                        if (pos == null)
                            return NotFound();

                        pos.КодСотрудника = dep.КодДиректораДепартамента;
                        pos.ДатаНазначения = DateTime.Now.Date;

                        db.ДолжностиСотрудниковs.Update(pos);
                        db.SaveChanges();
                    }

                    return RedirectToAction("Index");
                }
            }

            return View(dep);
        }

        public IActionResult DeletionForbidden()
        {
            return View();
        }

        [HttpGet]
        public IActionResult DeleteProject(int id = -1)
        {
            if (id == -1)
                return NotFound();

            using (var db = new CourseProject2DBContext())
            {
                var tableItem = (from item in db.Проектыs
                                 where item.Код == id
                                 select item).FirstOrDefault();

                if (tableItem == null)
                    return NotFound();

                var query = (from item in db.ПроектыИСотрудникиs
                             where item.КодПроекта == id
                             select item).Any();

                if (query)
                    return RedirectToAction("DeletionForbidden");

                return View(tableItem);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteProject(Проекты project)
        {
            using (var db = new CourseProject2DBContext())
            {
                if (ModelState.ErrorCount == 3)
                {
                    db.Проектыs.Remove(project);
                    db.SaveChanges();

                    return RedirectToAction("Index");
                }
            }

            return View(project);
        }

        [HttpGet]
        public IActionResult DeleteDepartment(int id = -1)
        {
            if (id == -1)
                return NotFound();

            using (var db = new CourseProject2DBContext())
            {
                var tableItem = (from item in db.Департаментыs
                                 where item.Код == id
                                 select item).FirstOrDefault();

                if (tableItem == null)
                    return NotFound();

                var query = (from item in db.ДолжностиСотрудниковs
                             where item.КодДепартамента == id
                             select item).Count();

                if (query > 1)
                    return RedirectToAction("DeletionForbidden");

                return View(tableItem);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteDepartment(Департаменты dep)
        {
            using (var db = new CourseProject2DBContext())
            {
                if (ModelState.ErrorCount == 1)
                {
                    var depDirector = (from dep_emp in db.ДолжностиСотрудниковs
                                       join pos in db.Должностиs on dep_emp.КодДолжности equals pos.Код
                                       where dep_emp.КодДепартамента == dep.Код && pos.Должность == "Директор департамента"
                                       select dep_emp).First();

                    db.ДолжностиСотрудниковs.Remove(depDirector);
                    db.Департаментыs.Remove(dep);
                    db.SaveChanges();

                    return RedirectToAction("Index");
                }
            }

            return View(dep);
        }
    }
}
