using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Practice.Data;
using Practice.Helper;
using Practice.Models;

namespace Practice.Controllers
{
    [Authorize(Roles = "Менеджер проекта")]
    public class ProjectManagerController : Controller
    {
        private static Dictionary<int, string> GetProjectManagerProjects(string login)
        {
            using (var db = new CourseProject2DBContext())
            {
                var projects = new Dictionary<int, string>();

                var query = from project in db.Проектыs
                            join emp in db.Сотрудникиs on project.КодМенеджераПроекта equals emp.Код
                            where emp.Логин == login && project.ДатаЗавершенияПроекта == null
                            orderby project.НазваниеПроекта
                            select new
                            {
                                Код = project.Код,
                                НазваниеПроекта = project.НазваниеПроекта
                            };

                foreach (var item in query)
                    projects.Add(item.Код, item.НазваниеПроекта);

                return projects;
            }
        }

        private static Dictionary<string, string> GetSelectedProject(int projectId)
        {
            using (var db = new CourseProject2DBContext())
            {
                var query = (from project in db.Проектыs
                             join emp in db.Сотрудникиs on project.КодМенеджераПроекта equals emp.Код
                             join prType in db.ТипыПроектовs on project.КодТипаПроекта equals prType.Код
                             where project.Код == projectId
                             select new
                             {
                                 Код = project.Код.ToString(),
                                 НазваниеПроекта = project.НазваниеПроекта,
                                 ДатаНачалаПроекта = project.ДатаНачалаПроекта.Date.ToString(),
                                 ДатаЗавершенияПроекта = project.ДатаЗавершенияПроекта.Value.Date.ToString(),
                                 КодМенеджераПроекта = project.КодМенеджераПроекта.ToString(),
                                 ФамилияМенеджераПроекта = emp.Фамилия,
                                 ИмяМенеджераПроекта = emp.Имя,
                                 ОтчествоМенеджераПроекта = emp.Отчество,
                                 ТипПроекта = prType.ТипПроекта
                             }).First();

                var json_pr = JsonConvert.SerializeObject(query);
                var dict_pr = JsonConvert.DeserializeObject<Dictionary<string, string>>(json_pr);

                return dict_pr;
            }
        }

        private static List<Dictionary<string, string>> GetProjectDevelopers(int projectId)
        {
            using (var db = new CourseProject2DBContext())
            {
                var devs = new List<Dictionary<string, string>>();

                var query = from pr_emp in db.ПроектыИСотрудникиs
                            join emp in db.Сотрудникиs on pr_emp.КодСотрудника equals emp.Код
                            join role in db.Ролиs on pr_emp.КодРоли equals role.Код
                            where pr_emp.КодПроекта == projectId && (pr_emp.ДатаОкончанияРаботыНаПроекте > DateTime.Now || pr_emp.ДатаОкончанияРаботыНаПроекте == null)
                            select new
                            {
                                Код = emp.Код,
                                Фамилия = emp.Фамилия,
                                Имя = emp.Имя,
                                Отчество = emp.Отчество,
                                Логин = emp.Логин,
                                Роль = role.Роль,
                                ДатаНачалаРаботыНаПроекте = pr_emp.ДатаНачалаРаботыНаПроекте.ToString("yyyy-MM-dd HH:mm:ss"),
                                ДатаОкончанияРаботыНаПроекте = pr_emp.ДатаОкончанияРаботыНаПроекте.Value.ToString("yyyy-MM-dd HH:mm:ss")
                            };

                var query1 = from rate in db.СтавкиСотрудниковs
                             where rate.КодПроекта == projectId && rate.ДатаНачалаДействияСтавки <= DateTime.Now
                             group rate by rate.КодСотрудника into g
                             select new
                             {
                                 Код = g.Select(a => a.Код).First(),
                                 КодСотрудника = g.Key,
                                 ДатаНачалаДействияСтавки = g.Max(s => s.ДатаНачалаДействияСтавки)
                             };

                var query2 = from item in query1
                             join rate in db.СтавкиСотрудниковs on item.Код equals rate.Код
                             select new
                             {
                                 КодСотрудника = item.КодСотрудника,
                                 ДатаНачалаДействияСтавки = item.ДатаНачалаДействияСтавки,
                                 Ставка = rate.Ставка
                             };

                var resultQuery = from emp in query
                                  join item2 in query2 on emp.Код equals item2.КодСотрудника into gr
                                  from g in gr.DefaultIfEmpty()
                                  select new
                                  {
                                      Код = emp.Код.ToString(),
                                      Фамилия = emp.Фамилия,
                                      Имя = emp.Имя,
                                      Отчество = emp.Отчество,
                                      Логин = emp.Логин,
                                      Роль = emp.Роль,
                                      ДатаНачалаРаботыНаПроекте = emp.ДатаНачалаРаботыНаПроекте,
                                      ДатаОкончанияРаботыНаПроекте = emp.ДатаОкончанияРаботыНаПроекте,
                                      ДатаНачалаДействияСтавки = g.ДатаНачалаДействияСтавки.ToString("yyyy-MM-dd HH:mm:ss") ?? string.Empty,
                                      Ставка = g.Ставка.ToString() ?? string.Empty
                                  };

                foreach (var item in resultQuery)
                {
                    var json_emps = JsonConvert.SerializeObject(item);
                    var dict_emps = JsonConvert.DeserializeObject<Dictionary<string, string>>(json_emps);

                    devs.Add(dict_emps);
                }

                if (devs.Count == 0)
                {
                    var list = new List<Dictionary<string, string>>();
                    var dict = new Dictionary<string, string>();
                    var keys = new string[] { "Код", "Фамилия", "Имя", "Отчество", "Логин", "Роль", "ДатаНачалаРаботыНаПроекте", "ДатаОкончанияРаботыНаПроекте", "ДатаНачалаДействияСтавки", "Ставка" };

                    foreach (var key in keys)
                        dict.Add(key, null);

                    list.Add(dict);

                    return list;
                }

                return devs;
            }
        }

        public IActionResult Index(int projectId = -1)
        {
            var projects = GetProjectManagerProjects(User.Identity.Name);

            if (projectId == -1)
                return View(new ProjectManagerPage() { projects = projects });
            else if (!projects.Keys.Contains(projectId))
                return NotFound();

            return View(new ProjectManagerPage() { projects = projects, selectedProject = GetSelectedProject(projectId), employees = GetProjectDevelopers(projectId) });
        }

        [HttpGet]
        public IActionResult AvailableEmployees(int projectId = -1)
        {
            if (projectId == -1)
                return NotFound();

            using (var db = new CourseProject2DBContext())
            {
                var devs = new List<Dictionary<string, string>>();
                List<long> projectDevs = new List<long>();
                var prDev = GetProjectDevelopers(projectId);

                if (prDev[0]["Код"] != null)
                {
                    foreach (var dev in GetProjectDevelopers(projectId))
                        projectDevs.Add(long.Parse(dev["Код"]));
                }

                var query = (from emp in db.Сотрудникиs
                             join sex in db.Полs on emp.КодПола equals sex.Код
                             join empType in db.ВидыТрудоустройстваs on emp.КодВидаТрудоустройства equals empType.Код
                             join pr_emp in db.ПроектыИСотрудникиs on emp.Код equals pr_emp.КодСотрудника into gr
                             from g in gr.DefaultIfEmpty()
                             where !projectDevs.Contains(emp.Код) && emp.Логин != User.Identity.Name
                             && (
                             from employee in db.УстройствоНаРаботуs
                             where employee.ДатаУвольнения > DateTime.Now.Date || employee.ДатаУвольнения == null
                             select employee.КодСотрудника).ToList().Contains(emp.Код)
                             select new
                             {
                                 Код = emp.Код.ToString(),
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
                             }).Distinct();

                foreach (var item in query)
                {
                    var json_emps = JsonConvert.SerializeObject(item);
                    var dict_emps = JsonConvert.DeserializeObject<Dictionary<string, string>>(json_emps);

                    devs.Add(dict_emps);
                }

                if (devs.Count == 0)
                {
                    var list = new List<Dictionary<string, string>>();
                    var dict = new Dictionary<string, string>();
                    var keys = new string[] { "Код", "Фамилия", "Имя", "Отчество", "Пол", "ДатаРождения", "Телефон1", "Телефон2", "ДатаНачалаРаботыВSap", "ВидТрудоустройства", "Логин" };

                    foreach (var key in keys)
                        dict.Add(key, null);

                    list.Add(dict);

                    return View(list);
                }

                ViewBag.ProjectId = projectId;

                return View(devs);
            }
        }

        [HttpGet]
        public IActionResult AddDeveloper(long id = -1, int projectId = -1)
        {
            if (id == -1 || projectId == -1)
                return NotFound();

            using (var db = new CourseProject2DBContext())
            {
                // является ли текущий пользователь менеджером данного проекта
                bool query = (from project in db.Проектыs
                              join emp in db.Сотрудникиs on project.КодМенеджераПроекта equals emp.Код
                              where project.Код == projectId && emp.Логин == User.Identity.Name
                              select emp).Any();

                if (!query)
                    return NotFound();

                List<long> projectDevs = new List<long>();
                var prDev = GetProjectDevelopers(projectId);

                if (prDev[0]["Код"] != null)
                {
                    foreach (var dev in GetProjectDevelopers(projectId))
                        projectDevs.Add(long.Parse(dev["Код"]));
                }

                // является ли сотрудник доступным
                var query1 = from emp in db.Сотрудникиs
                             where !projectDevs.Contains(emp.Код)
                             select emp.Код;

                if (!query1.Contains(id))
                    return NotFound();

                ViewBag.Id = id;
                ViewBag.ProjectId = projectId;

                return View(new ПроектыИСотрудники() { КодСотрудника = id, КодПроекта = projectId });
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult AddDeveloper(ПроектыИСотрудники newDev)
        {
            using (var db = new CourseProject2DBContext())
            {
                if (ModelState.ErrorCount == 3)
                {
                    db.ПроектыИСотрудникиs.Add(newDev);
                    db.SaveChanges();

                    return RedirectToAction("Index", new { projectId = newDev.КодПроекта });
                }
            }

            ViewBag.Id = newDev.Код;
            ViewBag.ProjectId = newDev.КодПроекта;

            return View(newDev);
        }

        [HttpGet]
        public IActionResult GetAllTasks(int projectId)
        {
            using (var db = new CourseProject2DBContext())
            {
                // является ли текущий пользователь менеджером данного проекта
                bool query = (from project in db.Проектыs
                              join emp in db.Сотрудникиs on project.КодМенеджераПроекта equals emp.Код
                              where project.Код == projectId && emp.Логин == User.Identity.Name
                              select emp).Any();

                if (!query)
                    return NotFound();

                var tasks = new List<Dictionary<string, string>>();

                var query1 = from task in db.ФактическиеТрудозатратыs
                             join status in db.Статусыs on task.КодСтатуса equals status.Код
                             join emp in db.Сотрудникиs on task.КодРазработчика equals emp.Код
                             where task.КодПроекта == projectId
                             orderby status.Статус, task.ПоследнееИзменение descending
                             select new
                             {
                                 Код = task.Код,
                                 КодРазработчика = task.КодРазработчика,
                                 Фамилия = emp.Фамилия,
                                 Имя = emp.Имя,
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
                    var keys = new string[] { "Код", "КодРазработчика", "Фамилия", "Имя", "Задача",  
                        "ДатаТрудозатраты", "КоличествоЧасов", "Комментарий", "Статус", "ПоследнееИзменение" };
                    var dict = new Dictionary<string, string>();

                    foreach (var key in keys)
                        dict.Add(key, null);

                    tasks.Add(dict);
                }

                ViewBag.ProjectId = projectId;

                return View(tasks);
            }
        }

        [HttpGet]
        public IActionResult CheckTask(int projectId, long taskId, int statusId)
        {
            using (var db = new CourseProject2DBContext())
            {
                // корректный ли статус
                bool query = (from status in db.Статусыs
                              where status.Код == statusId
                              select status).Any();

                if (!query)
                    return NotFound();

                // существует ли такая трудозатрата и верно ли указан проект
                var query1 = (from task in db.ФактическиеТрудозатратыs
                              where task.Код == taskId && task.КодПроекта == projectId
                              select task).FirstOrDefault();

                if (query1 == null)
                    return NotFound();

                // является ли текущий пользователь менеджером данного проекта
                bool query2 = (from project in db.Проектыs
                               join emp in db.Сотрудникиs on project.КодМенеджераПроекта equals emp.Код
                               where project.Код == projectId && emp.Логин == User.Identity.Name
                               select emp).Any();

                if (!query2)
                    return NotFound();

                query1.КодСтатуса = statusId;

                return View(query1);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult CheckTask(ФактическиеТрудозатраты newTask)
        {
            using (var db = new CourseProject2DBContext())
            {
                if (ModelState.ErrorCount == 3)
                {
                    newTask.ПоследнееИзменение = DateTime.Now;

                    db.ФактическиеТрудозатратыs.Update(newTask);
                    db.SaveChanges();

                    return RedirectToAction("GetAllTasks", new { projectId = newTask.КодПроекта });
                }
            }

            return View(newTask);
        }

        [HttpGet]
        public IActionResult UpdateDeveloper(long id = -1, int projectId = -1)
        {
            if (id == -1 || projectId == -1)
                return NotFound();

            using (var db = new CourseProject2DBContext())
            {
                // является ли текущий пользователь менеджером данного проекта
                bool query = (from project in db.Проектыs
                              join emp in db.Сотрудникиs on project.КодМенеджераПроекта equals emp.Код
                              where project.Код == projectId && emp.Логин == User.Identity.Name
                              select emp).Any();

                if (!query)
                    return NotFound();

                // является ли сотрудник разработчиком проекта в данный момент
                bool query1 = (from pr_emp in db.ПроектыИСотрудникиs
                               where pr_emp.КодСотрудника == id && pr_emp.КодПроекта == projectId
                               && (pr_emp.ДатаОкончанияРаботыНаПроекте > DateTime.Now.Date || pr_emp.ДатаОкончанияРаботыНаПроекте == null)
                               select pr_emp).Any();

                if (!query1)
                    return NotFound();

                var developer = new Dictionary<string, string>();

                var query2 = from emp in db.Сотрудникиs
                             join sex in db.Полs on emp.КодПола equals sex.Код
                             join empType in db.ВидыТрудоустройстваs on emp.КодВидаТрудоустройства equals empType.Код
                             join pr_emp in db.ПроектыИСотрудникиs on emp.Код equals pr_emp.КодСотрудника
                             join role in db.Ролиs on pr_emp.КодРоли equals role.Код
                             where emp.Код == id && pr_emp.КодПроекта == projectId
                             && (pr_emp.ДатаОкончанияРаботыНаПроекте > DateTime.Now.Date || pr_emp.ДатаОкончанияРаботыНаПроекте == null)
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
                                 Логин = emp.Логин,
                                 Роль = role.Роль,
                                 ДатаНачалаРаботыНаПроекте = pr_emp.ДатаНачалаРаботыНаПроекте.Date.ToString(),
                                 ДатаОкончанияРаботыНаПроекте = pr_emp.ДатаОкончанияРаботыНаПроекте.Value.Date.ToString()
                             };

                var query3 = from rate in db.СтавкиСотрудниковs
                             where rate.КодСотрудника == id && rate.КодПроекта == projectId && rate.ДатаНачалаДействияСтавки <= DateTime.Now
                             group rate by rate.КодСотрудника into g
                             select new
                             {
                                 Код = g.Select(a => a.Код).First(),
                                 КодСотрудника = g.Key,
                                 ДатаНачалаДействияСтавки = g.Max(s => s.ДатаНачалаДействияСтавки)
                             };

                var query4 = from item in query3
                             join rate in db.СтавкиСотрудниковs on item.Код equals rate.Код
                             select new
                             {
                                 КодСотрудника = item.КодСотрудника,
                                 ДатаНачалаДействияСтавки = item.ДатаНачалаДействияСтавки,
                                 Ставка = rate.Ставка
                             };

                var json_emps1 = JsonConvert.SerializeObject(query2.FirstOrDefault());
                developer = JsonConvert.DeserializeObject<Dictionary<string, string>>(json_emps1);

                var query5 = (from emp in query2
                              join rate in query4 on emp.Код equals rate.КодСотрудника into gr
                              from g in gr.DefaultIfEmpty()
                              select new
                              {
                                  Код = emp.Код,
                                  Фамилия = emp.Фамилия,
                                  Имя = emp.Имя,
                                  Отчество = emp.Отчество,
                                  Пол = emp.Пол,
                                  ДатаРождения = emp.ДатаРождения,
                                  Телефон1 = emp.Телефон1,
                                  Телефон2 = emp.Телефон2,
                                  ДатаНачалаРаботыВSap = emp.ДатаНачалаРаботыВSap,
                                  ВидТрудоустройства = emp.ВидТрудоустройства,
                                  Логин = emp.Логин,
                                  ДатаНачалаРаботыНаПроекте = emp.ДатаНачалаРаботыНаПроекте,
                                  Роль = emp.Роль,
                                  ДатаНачалаДействияСтавки = g.ДатаНачалаДействияСтавки.ToString("yyyy-MM-dd HH:mm:ss") ?? string.Empty,
                                  Ставка = g.Ставка.ToString() ?? string.Empty,
                                  ДатаОкончанияРаботыНаПроекте = emp.ДатаОкончанияРаботыНаПроекте
                              }).FirstOrDefault();

                if (query5 == null)
                    return NotFound();

                var json_emps = JsonConvert.SerializeObject(query5);
                developer = JsonConvert.DeserializeObject<Dictionary<string, string>>(json_emps);

                ViewBag.ProjectId = projectId;

                return View(developer);
            }
        }

        [HttpGet]
        public IActionResult UpdateRole(long id = -1, int projectId = -1)
        {
            if (id == -1 || projectId == -1)
                return NotFound();

            using (var db = new CourseProject2DBContext())
            {
                // является ли текущий пользователь менеджером данного проекта
                bool query = (from project in db.Проектыs
                              join emp in db.Сотрудникиs on project.КодМенеджераПроекта equals emp.Код
                              where project.Код == projectId && emp.Логин == User.Identity.Name
                              select emp).Any();

                if (!query)
                    return NotFound();

                // является ли сотрудник разработчиком проекта в данный момент
                bool query1 = (from pr_emp in db.ПроектыИСотрудникиs
                               where pr_emp.КодСотрудника == id && pr_emp.КодПроекта == projectId
                               && (pr_emp.ДатаОкончанияРаботыНаПроекте > DateTime.Now.Date || pr_emp.ДатаОкончанияРаботыНаПроекте == null)
                               select pr_emp).Any();

                if (!query1)
                    return NotFound();

                // является ли текущий пользователь менеджером данного проекта
                bool query2 = (from project in db.Проектыs
                               join emp in db.Сотрудникиs on project.КодМенеджераПроекта equals emp.Код
                               where project.Код == projectId && emp.Логин == User.Identity.Name
                               select emp).Any();

                if (!query2)
                    return NotFound();

                var query3 = (from pr_emp in db.ПроектыИСотрудникиs
                              where pr_emp.КодПроекта == projectId && pr_emp.КодСотрудника == id
                              select pr_emp).FirstOrDefault();

                if (query3 == null)
                    return NotFound();

                return View(query3);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult UpdateRole(ПроектыИСотрудники emp)
        {
            using (var db = new CourseProject2DBContext())
            {
                if (ModelState.ErrorCount == 3)
                {
                    db.ПроектыИСотрудникиs.Update(emp);
                    db.SaveChanges();

                    return RedirectToAction("UpdateDeveloper", new { id = emp.КодСотрудника, projectId = emp.КодПроекта });
                }
            }

            return View(emp);
        }

        [HttpGet]
        public IActionResult UpdateEndDate(long id = -1, int projectId = -1)
        {
            if (id == -1 || projectId == -1)
                return NotFound();

            using (var db = new CourseProject2DBContext())
            {
                // является ли текущий пользователь менеджером данного проекта
                bool query = (from project in db.Проектыs
                              join emp in db.Сотрудникиs on project.КодМенеджераПроекта equals emp.Код
                              where project.Код == projectId && emp.Логин == User.Identity.Name
                              select emp).Any();

                if (!query)
                    return NotFound();

                // является ли сотрудник разработчиком проекта в данный момент
                bool query1 = (from pr_emp in db.ПроектыИСотрудникиs
                               where pr_emp.КодСотрудника == id && pr_emp.КодПроекта == projectId
                               && (pr_emp.ДатаОкончанияРаботыНаПроекте > DateTime.Now.Date || pr_emp.ДатаОкончанияРаботыНаПроекте == null)
                               select pr_emp).Any();

                if (!query1)
                    return NotFound();

                // является ли текущий пользователь менеджером данного проекта
                bool query2 = (from project in db.Проектыs
                               join emp in db.Сотрудникиs on project.КодМенеджераПроекта equals emp.Код
                               where project.Код == projectId && emp.Логин == User.Identity.Name
                               select emp).Any();

                if (!query2)
                    return NotFound();

                var query3 = (from pr_emp in db.ПроектыИСотрудникиs
                              where pr_emp.КодПроекта == projectId && pr_emp.КодСотрудника == id
                              select pr_emp).FirstOrDefault();

                if (query3 == null)
                    return NotFound();

                return View(query3);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult UpdateEndDate(ПроектыИСотрудники emp)
        {
            using (var db = new CourseProject2DBContext())
            {
                if (ModelState.ErrorCount == 3)
                {
                    db.ПроектыИСотрудникиs.Update(emp);
                    db.SaveChanges();

                    if (emp.ДатаОкончанияРаботыНаПроекте > DateTime.Now || emp.ДатаОкончанияРаботыНаПроекте == null)
                        return RedirectToAction("UpdateDeveloper", new { id = emp.КодСотрудника, projectId = emp.КодПроекта });
                    else
                        return RedirectToAction("Index", new { projectId = emp.КодПроекта });
                }
            }

            return View(emp);
        }

        [HttpGet]
        public IActionResult UpdateRate(long id = -1, int projectId = -1)
        {
            if (id == -1 || projectId == -1)
                return NotFound();

            using (var db = new CourseProject2DBContext())
            {
                // является ли текущий пользователь менеджером данного проекта
                bool query = (from project in db.Проектыs
                              join emp in db.Сотрудникиs on project.КодМенеджераПроекта equals emp.Код
                              where project.Код == projectId && emp.Логин == User.Identity.Name
                              select emp).Any();

                if (!query)
                    return NotFound();

                // является ли сотрудник разработчиком проекта в данный момент
                bool query1 = (from pr_emp in db.ПроектыИСотрудникиs
                               where pr_emp.КодСотрудника == id && pr_emp.КодПроекта == projectId
                               && (pr_emp.ДатаОкончанияРаботыНаПроекте > DateTime.Now.Date || pr_emp.ДатаОкончанияРаботыНаПроекте == null)
                               select pr_emp).Any();

                if (!query1)
                    return NotFound();

                // является ли текущий пользователь менеджером данного проекта
                bool query2 = (from project in db.Проектыs
                               join emp in db.Сотрудникиs on project.КодМенеджераПроекта equals emp.Код
                               where project.Код == projectId && emp.Логин == User.Identity.Name
                               select emp).Any();

                if (!query2)
                    return NotFound();

                return View(new СтавкиСотрудников() { КодСотрудника = id, КодПроекта = projectId });
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult UpdateRate(СтавкиСотрудников empRate)
        {
            using (var db = new CourseProject2DBContext())
            {
                var query1 = from rate in db.СтавкиСотрудниковs
                             where rate.КодСотрудника == empRate.КодСотрудника && rate.КодПроекта == empRate.КодПроекта
                             group rate by rate.КодСотрудника into g
                             select g.Max(s => s.ДатаНачалаДействияСтавки);

                if (query1.Count() != 0 && query1.First() >= empRate.ДатаНачалаДействияСтавки)
                    ModelState.AddModelError("ДатаНачалаДействияСтавки", $"Ошибка! Дата начала действия предыдущей ставки: {query1}. Дата начала действия новой ставки должна быть больше!");

                if (ModelState.ErrorCount == 2)
                {
                    db.СтавкиСотрудниковs.Add(empRate);
                    db.SaveChanges();

                    return RedirectToAction("UpdateDeveloper", new { id = empRate.КодСотрудника, projectId = empRate.КодПроекта });
                }
            }

            return View(empRate);
        }
    }
}
