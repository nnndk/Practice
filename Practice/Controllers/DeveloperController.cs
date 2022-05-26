using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Practice.Data;
using Practice.Helper;
using Practice.Models;
using System.Diagnostics;

namespace Practice.Controllers
{
    [Authorize(Roles = "Разработчик")]
    public class DeveloperController : Controller
    {
        private static Dictionary<int, string> GetEmployeeProjects(string login)
        {
            using (var db = new CourseProject2DBContext())
            {
                var projects = new Dictionary<int, string>();

                var query = from pr_emp in db.ПроектыИСотрудникиs
                            join project in db.Проектыs on pr_emp.КодПроекта equals project.Код
                            join emp in db.Сотрудникиs on pr_emp.КодСотрудника equals emp.Код
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

        private static List<Dictionary<string, string>> GetEmployeeTasks(int projectId)
        {
            using (var db = new CourseProject2DBContext())
            {
                var tasks = new List<Dictionary<string, string>>();

                var query = from task in db.ФактическиеТрудозатратыs
                            join status in db.Статусыs on task.КодСтатуса equals status.Код
                            where task.КодПроекта == projectId
                            orderby task.ПоследнееИзменение descending
                            select new
                            {
                                Код = task.Код.ToString(),
                                Задача = task.Задача,
                                ДатаТрудозатраты = task.ДатаТрудозатраты.Date.ToString(),
                                КоличествоЧасов = task.КоличествоЧасов.ToString(),
                                Статус = status.Статус,
                                Комментарий = task.Комментарий,
                                ПоследнееИзменение = task.ПоследнееИзменение.ToString("yyyy-MM-dd HH:mm:ss")
                            };

                foreach (var item in query)
                {
                    var json_task = JsonConvert.SerializeObject(item);
                    var dict_task = JsonConvert.DeserializeObject<Dictionary<string, string>>(json_task);

                    tasks.Add(dict_task);
                }

                if (tasks.Count == 0)
                {
                    var list = new List<Dictionary<string, string>>();
                    var dict = new Dictionary<string, string>();
                    var keys = new string[] { "Код", "Задача", "ДатаТрудозатраты", "КоличествоЧасов", "Статус", "Комментарий", "ПоследнееИзменение" };

                    foreach (var key in keys)
                        dict.Add(key, null);

                    list.Add(dict);

                    return list;
                }

                return tasks;
            }
        }

        [HttpGet]
        public IActionResult Index(int id = -1)
        {
            var projects = GetEmployeeProjects(User.Identity.Name);

            if (id == -1)
                return View(new DeveloperPage() { projects = projects });
            else if (!projects.Keys.Contains(id))
                return NotFound();

            return View(new DeveloperPage() { projects = projects, selectedProject = GetSelectedProject(id), tasks = GetEmployeeTasks(id) });
        }

        public IActionResult BlockedProjectError()
        {
            return View();
        }

        [HttpGet]
        public IActionResult AddTask(int id = -1)
        {
            if (id == -1)
                return NotFound();

            ViewBag.ProjectId = id;

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult AddTask(ФактическиеТрудозатраты task)
        {
            using (var db = new CourseProject2DBContext())
            {
                if (ModelState.ErrorCount == 3)
                {
                    long empId = (from emp in db.Сотрудникиs
                                  where emp.Логин == User.Identity.Name
                                  select emp.Код).First();

                    task.КодРазработчика = empId;
                    task.КодСтатуса = 3;
                    task.ПоследнееИзменение = DateTime.Now;

                    db.ФактическиеТрудозатратыs.Add(task);
                    db.SaveChanges();

                    return RedirectToAction("Index", new { id = task.КодПроекта });
                }
            }

            return View(task);
        }

        [HttpGet]
        public IActionResult UpdateTask(long id = -1)
        {
            if (id == -1)
                return NotFound();

            using (var db = new CourseProject2DBContext())
            {
                var task = db.ФактическиеТрудозатратыs.Find(id);

                if (task == null)
                    return NotFound();

                ViewBag.ProjectId = task.КодПроекта;
                ViewBag.TaskId = task.Код;

                return View(task);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult UpdateTask(ФактическиеТрудозатраты task)
        {
            using (var db = new CourseProject2DBContext())
            {
                if (ModelState.ErrorCount == 3)
                {
                    long empId = (from emp in db.Сотрудникиs
                                  where emp.Логин == User.Identity.Name
                                  select emp.Код).First();

                    task.КодРазработчика = empId;
                    task.КодСтатуса = 3;
                    task.ПоследнееИзменение = DateTime.Now;

                    db.ФактическиеТрудозатратыs.Update(task);
                    db.SaveChanges();
                    TempData["success"] = "Category updated successfully";

                    return RedirectToAction("Index", new { id = task.КодПроекта });
                }
            }

            return View(task);
        }

        [HttpGet]
        public IActionResult DeleteTask(long id = -1)
        {
            if (id == -1)
                return NotFound();

            using (var db = new CourseProject2DBContext())
            {
                var task = db.ФактическиеТрудозатратыs.Find(id);

                if (task == null)
                    return NotFound();

                ViewBag.ProjectId = task.КодПроекта;
                ViewBag.TaskId = task.Код;

                return View(task);
            }
        }

        [HttpPost, ActionName("DeleteTask")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteTaskPOST(long id = -1)
        {
            using (var db = new CourseProject2DBContext())
            {
                var task = db.ФактическиеТрудозатратыs.Find(id);

                if (task == null)
                    return NotFound();

                db.ФактическиеТрудозатратыs.Remove(task);
                db.SaveChanges();
                TempData["success"] = "Category deleted successfully";

                return RedirectToAction("Index", new { id = task.КодПроекта });
            }
        }
    }
}
