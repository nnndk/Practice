using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Practice.Data;
using Practice.Helper;
using Practice.Models;

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
                                 ДатаНачалаПроекта = project.ДатаНачалаПроекта.Date.ToString("yyyy-MM-dd HH:mm:ss"),
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
                                 ДатаРождения = emp.ДатаРождения.Date.ToString("yyyy-MM-dd HH:mm:ss"),
                                 Телефон1 = emp.Телефон1,
                                 Телефон2 = emp.Телефон2,
                                 ДатаНачалаРаботыВSap = emp.ДатаНачалаРаботыВSap.Value.Date.ToString("yyyy-MM-dd HH:mm:ss"),
                                 ВидТрудоустройства = empType.ВидТрудоустройства,
                                 Логин = emp.Логин,
                                 ДатаЗачисленияВШтат = employment.ДатаЗачисленияВШтат.Date.ToString("yyyy-MM-dd HH:mm:ss"),
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

                return View(new ContractDepEmpPage() { projects = projects, employees = emps });
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
                if (ModelState.ErrorCount == 2)
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
                               select pr).First();

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
                                 ДатаРождения = emp.ДатаРождения.Date.ToString("yyyy-MM-dd HH:mm:ss"),
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
                             select employment).First();

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
                                 ДатаТрудозатраты = task.ДатаТрудозатраты.ToString("yyyy-MM-dd HH:mm:ss"),
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
    }
}
