using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Practice.Models;
using Practice.Data;
using Newtonsoft.Json;

namespace Practice.Controllers
{
    [Authorize(Roles = "Администратор баз данных")]
    public class DbAdminController : Controller
    {
        public IActionResult Index(string tableName = "")
        {
            if (tableName != "")
            {
                var tables = new string[] { "Виды трудоустройства", "Должности", "Пол", "Роли", "Статусы", "Типы проектов" };

                if (!tables.Contains(tableName))
                    return NotFound();

                var table = new List<Dictionary<string, string>>();

                using (var db = new CourseProject2DBContext())
                {
                    switch (tableName)
                    {
                        case "Виды трудоустройства":
                            var query1 = from item in db.ВидыТрудоустройстваs
                                         select new
                                         {
                                             item.Код,
                                             item.ВидТрудоустройства
                                         };

                            foreach (var item in query1)
                            {
                                var json = JsonConvert.SerializeObject(item);
                                table.Add(JsonConvert.DeserializeObject<Dictionary<string, string>>(json));
                            }

                            if (table.Count == 0)
                                table.Add(new Dictionary<string, string>() { { "Код", null }, { "ВидТрудоустройства", null } });
                            break;
                        case "Должности":
                            var query2 = from item in db.Должностиs
                                         select new
                                         {
                                             item.Код,
                                             item.Должность
                                         };

                            foreach (var item in query2)
                            {
                                var json = JsonConvert.SerializeObject(item);
                                table.Add(JsonConvert.DeserializeObject<Dictionary<string, string>>(json));
                            }

                            if (table.Count == 0)
                                table.Add(new Dictionary<string, string>() { { "Код", null }, { "Должность", null } });
                            break;
                        case "Пол":
                            var query3 = from item in db.Полs
                                         select new
                                         {
                                             item.Код,
                                             Пол = item.Пол1
                                         };

                            foreach (var item in query3)
                            {
                                var json = JsonConvert.SerializeObject(item);
                                table.Add(JsonConvert.DeserializeObject<Dictionary<string, string>>(json));
                            }

                            if (table.Count == 0)
                                table.Add(new Dictionary<string, string>() { { "Код", null }, { "Пол", null } });
                            break;
                        case "Роли":
                            var query4 = from item in db.Ролиs
                                         select new
                                         {
                                             item.Код,
                                             item.Роль
                                         };

                            foreach (var item in query4)
                            {
                                var json = JsonConvert.SerializeObject(item);
                                table.Add(JsonConvert.DeserializeObject<Dictionary<string, string>>(json));
                            }

                            if (table.Count == 0)
                                table.Add(new Dictionary<string, string>() { { "Код", null }, { "Роль", null } });
                            break;
                        case "Статусы":
                            var query5 = from item in db.Статусыs
                                         select new
                                         {
                                             item.Код,
                                             item.Статус
                                         };

                            foreach (var item in query5)
                            {
                                var json = JsonConvert.SerializeObject(item);
                                table.Add(JsonConvert.DeserializeObject<Dictionary<string, string>>(json));
                            }

                            if (table.Count == 0)
                                table.Add(new Dictionary<string, string>() { { "Код", null }, { "Статус", null } });
                            break;
                        case "Типы проектов":
                            var query6 = from item in db.ТипыПроектовs
                                         select new
                                         {
                                             item.Код,
                                             item.ТипПроекта
                                         };

                            foreach (var item in query6)
                            {
                                var json = JsonConvert.SerializeObject(item);
                                table.Add(JsonConvert.DeserializeObject<Dictionary<string, string>>(json));
                            }

                            if (table.Count == 0)
                                table.Add(new Dictionary<string, string>() { { "Код", null }, { "ТипПроекта", null } });
                            break;
                    }

                    ViewBag.TableName = tableName;

                    return View(table);
                }
            }

            return View();
        }

        public IActionResult AddItem(string tableName = "")
        {
            switch (tableName)
            {
                case "Виды трудоустройства":
                    return RedirectToAction("AddEmpType");
                case "Должности":
                    return RedirectToAction("AddPosition");
                case "Пол":
                    return RedirectToAction("AddGender");
                case "Роли":
                    return RedirectToAction("AddRole");
                case "Статусы":
                    return RedirectToAction("AddStatus");
                case "Типы проектов":
                    return RedirectToAction("AddProjectType");
                default:
                    return NotFound();
            }
        }

        [HttpGet]
        public IActionResult AddEmpType()
        {
            return View(new ВидыТрудоустройства());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult AddEmpType(ВидыТрудоустройства empType)
        {
            using (var db = new CourseProject2DBContext())
            {
                if (ModelState.ErrorCount == 0)
                {
                    db.ВидыТрудоустройстваs.Add(empType);
                    db.SaveChanges();

                    return RedirectToAction("Index", new { tableName = "Виды трудоустройства" });
                }
            }

            return View(empType);
        }

        [HttpGet]
        public IActionResult AddPosition()
        {
            return View(new Должности());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult AddPosition(Должности pos)
        {
            using (var db = new CourseProject2DBContext())
            {
                if (ModelState.ErrorCount == 0)
                {
                    db.Должностиs.Add(pos);
                    db.SaveChanges();

                    return RedirectToAction("Index", new { tableName = "Должности" });
                }
            }

            return View(pos);
        }

        [HttpGet]
        public IActionResult AddGender()
        {
            return View(new Пол());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult AddGender(Пол sex)
        {
            using (var db = new CourseProject2DBContext())
            {
                if (ModelState.ErrorCount == 0)
                {
                    db.Полs.Add(sex);
                    db.SaveChanges();

                    return RedirectToAction("Index", new { tableName = "Пол" });
                }
            }

            return View(sex);
        }

        [HttpGet]
        public IActionResult AddRole()
        {
            return View(new Роли());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult AddRole(Роли role)
        {
            using (var db = new CourseProject2DBContext())
            {
                if (ModelState.ErrorCount == 0)
                {
                    db.Ролиs.Add(role);
                    db.SaveChanges();

                    return RedirectToAction("Index", new { tableName = "Роли" });
                }
            }

            return View(role);
        }

        [HttpGet]
        public IActionResult AddStatus()
        {
            return View(new Статусы());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult AddStatus(Статусы status)
        {
            using (var db = new CourseProject2DBContext())
            {
                if (ModelState.ErrorCount == 0)
                {
                    db.Статусыs.Add(status);
                    db.SaveChanges();

                    return RedirectToAction("Index", new { tableName = "Статусы" });
                }
            }

            return View(status);
        }

        [HttpGet]
        public IActionResult AddProjectType()
        {
            return View(new ТипыПроектов());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult AddProjectType(ТипыПроектов prType)
        {
            using (var db = new CourseProject2DBContext())
            {
                if (ModelState.ErrorCount == 0)
                {
                    db.ТипыПроектовs.Add(prType);
                    db.SaveChanges();

                    return RedirectToAction("Index", new { tableName = "Типы проектов" });
                }
            }

            return View(prType);
        }

        public IActionResult UpdateItem(string tableName = "", int id = -1)
        {
            if (id == -1)
                return NotFound();

            switch (tableName)
            {
                case "Виды трудоустройства":
                    return RedirectToAction("UpdateEmpType", new { id = id });
                case "Должности":
                    return RedirectToAction("UpdatePosition", new { id = id });
                case "Пол":
                    return RedirectToAction("UpdateGender", new { id = id });
                case "Роли":
                    return RedirectToAction("UpdateRole", new { id = id });
                case "Статусы":
                    return RedirectToAction("UpdateStatus", new { id = id });
                case "Типы проектов":
                    return RedirectToAction("UpdateProjectType", new { id = id });
                default:
                    return NotFound();
            }
        }

        [HttpGet]
        public IActionResult UpdateEmpType(int id = -1)
        {
            if (id == -1)
                return NotFound();

            using (var db = new CourseProject2DBContext())
            {
                var tableItem = (from item in db.ВидыТрудоустройстваs
                                 where item.Код == id
                                 select item).FirstOrDefault();

                if (tableItem == null)
                    return NotFound();

                return View(tableItem);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult UpdateEmpType(ВидыТрудоустройства empType)
        {
            using (var db = new CourseProject2DBContext())
            {
                if (ModelState.ErrorCount == 0)
                {
                    db.ВидыТрудоустройстваs.Update(empType);
                    db.SaveChanges();

                    return RedirectToAction("Index", new { tableName = "Виды трудоустройства" });
                }
            }

            return View(empType);
        }

        [HttpGet]
        public IActionResult UpdatePosition(int id = -1)
        {
            if (id == -1)
                return NotFound();

            using (var db = new CourseProject2DBContext())
            {
                var tableItem = (from item in db.Должностиs
                                 where item.Код == id
                                 select item).FirstOrDefault();

                if (tableItem == null)
                    return NotFound();

                return View(tableItem);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult UpdatePosition(Должности pos)
        {
            using (var db = new CourseProject2DBContext())
            {
                if (ModelState.ErrorCount == 0)
                {
                    db.Должностиs.Update(pos);
                    db.SaveChanges();

                    return RedirectToAction("Index", new { tableName = "Должности" });
                }
            }

            return View(pos);
        }

        [HttpGet]
        public IActionResult UpdateGender(int id = -1)
        {
            if (id == -1)
                return NotFound();

            using (var db = new CourseProject2DBContext())
            {
                var tableItem = (from item in db.Полs
                                 where item.Код == id
                                 select item).FirstOrDefault();

                if (tableItem == null)
                    return NotFound();

                return View(tableItem);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult UpdateGender(Пол sex)
        {
            using (var db = new CourseProject2DBContext())
            {
                if (ModelState.ErrorCount == 0)
                {
                    db.Полs.Update(sex);
                    db.SaveChanges();

                    return RedirectToAction("Index", new { tableName = "Пол" });
                }
            }

            return View(sex);
        }

        [HttpGet]
        public IActionResult UpdateRole(int id = -1)
        {
            if (id == -1)
                return NotFound();

            using (var db = new CourseProject2DBContext())
            {
                var tableItem = (from item in db.Ролиs
                                 where item.Код == id
                                 select item).FirstOrDefault();

                if (tableItem == null)
                    return NotFound();

                return View(tableItem);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult UpdateRole(Роли role)
        {
            using (var db = new CourseProject2DBContext())
            {
                if (ModelState.ErrorCount == 0)
                {
                    db.Ролиs.Update(role);
                    db.SaveChanges();

                    return RedirectToAction("Index", new { tableName = "Роли" });
                }
            }

            return View(role);
        }

        [HttpGet]
        public IActionResult UpdateStatus(int id = -1)
        {
            if (id == -1)
                return NotFound();

            using (var db = new CourseProject2DBContext())
            {
                var tableItem = (from item in db.Статусыs
                                 where item.Код == id
                                 select item).FirstOrDefault();

                if (tableItem == null)
                    return NotFound();

                return View(tableItem);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult UpdateStatus(Статусы status)
        {
            using (var db = new CourseProject2DBContext())
            {
                if (ModelState.ErrorCount == 0)
                {
                    db.Статусыs.Update(status);
                    db.SaveChanges();

                    return RedirectToAction("Index", new { tableName = "Статусы" });
                }
            }

            return View(status);
        }

        [HttpGet]
        public IActionResult UpdateProjectType(int id = -1)
        {
            if (id == -1)
                return NotFound();

            using (var db = new CourseProject2DBContext())
            {
                var tableItem = (from item in db.ТипыПроектовs
                                 where item.Код == id
                                 select item).FirstOrDefault();

                if (tableItem == null)
                    return NotFound();

                return View(tableItem);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult UpdateProjectType(ТипыПроектов prType)
        {
            using (var db = new CourseProject2DBContext())
            {
                if (ModelState.ErrorCount == 0)
                {
                    db.ТипыПроектовs.Update(prType);
                    db.SaveChanges();

                    return RedirectToAction("Index", new { tableName = "Типы проектов" });
                }
            }

            return View(prType);
        }

        public IActionResult DeleteItem(string tableName = "", int id = -1)
        {
            if (id == -1)
                return NotFound();

            switch (tableName)
            {
                case "Виды трудоустройства":
                    return RedirectToAction("DeleteEmpType", new { id = id });
                case "Должности":
                    return RedirectToAction("DeletePosition", new { id = id });
                case "Пол":
                    return RedirectToAction("DeleteGender", new { id = id });
                case "Роли":
                    return RedirectToAction("DeleteRole", new { id = id });
                case "Статусы":
                    return RedirectToAction("DeleteStatus", new { id = id });
                case "Типы проектов":
                    return RedirectToAction("DeleteProjectType", new { id = id });
                default:
                    return NotFound();
            }
        }

        public IActionResult DeletionForbidden()
        {
            return View();
        }

        [HttpGet]
        public IActionResult DeleteEmpType(int id = -1)
        {
            if (id == -1)
                return NotFound();

            using (var db = new CourseProject2DBContext())
            {
                var tableItem = (from item in db.ВидыТрудоустройстваs
                                 where item.Код == id
                                 select item).FirstOrDefault();

                if (tableItem == null)
                    return NotFound();

                var query = (from item in db.Сотрудникиs
                            where item.КодВидаТрудоустройства == id
                            select item).Any();

                if (query)
                    return RedirectToAction("DeletionForbidden");

                return View(tableItem);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteEmpType(ВидыТрудоустройства empType)
        {
            using (var db = new CourseProject2DBContext())
            {
                if (ModelState.ErrorCount == 0)
                {
                    db.ВидыТрудоустройстваs.Remove(empType);
                    db.SaveChanges();

                    return RedirectToAction("Index", new { tableName = "Виды трудоустройства" });
                }
            }

            return View(empType);
        }

        [HttpGet]
        public IActionResult DeletePosition(int id = -1)
        {
            if (id == -1)
                return NotFound();

            using (var db = new CourseProject2DBContext())
            {
                var tableItem = (from item in db.Должностиs
                                 where item.Код == id
                                 select item).FirstOrDefault();

                if (tableItem == null)
                    return NotFound();

                var query = (from item in db.ДолжностиСотрудниковs
                             where item.КодДолжности == id
                             select item).Any();

                if (query)
                    return RedirectToAction("DeletionForbidden");

                return View(tableItem);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult DeletePosition(Должности pos)
        {
            using (var db = new CourseProject2DBContext())
            {
                if (ModelState.ErrorCount == 0)
                {
                    db.Должностиs.Remove(pos);
                    db.SaveChanges();

                    return RedirectToAction("Index", new { tableName = "Должности" });
                }
            }

            return View(pos);
        }

        [HttpGet]
        public IActionResult DeleteGender(int id = -1)
        {
            if (id == -1)
                return NotFound();

            using (var db = new CourseProject2DBContext())
            {
                var tableItem = (from item in db.Полs
                                 where item.Код == id
                                 select item).FirstOrDefault();

                if (tableItem == null)
                    return NotFound();

                var query = (from item in db.Сотрудникиs
                             where item.КодПола == id
                             select item).Any();

                if (query)
                    return RedirectToAction("DeletionForbidden");

                return View(tableItem);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteGender(Пол sex)
        {
            using (var db = new CourseProject2DBContext())
            {
                if (ModelState.ErrorCount == 0)
                {
                    db.Полs.Remove(sex);
                    db.SaveChanges();

                    return RedirectToAction("Index", new { tableName = "Пол" });
                }
            }

            return View(sex);
        }

        [HttpGet]
        public IActionResult DeleteRole(int id = -1)
        {
            if (id == -1)
                return NotFound();

            using (var db = new CourseProject2DBContext())
            {
                var tableItem = (from item in db.Ролиs
                                 where item.Код == id
                                 select item).FirstOrDefault();

                if (tableItem == null)
                    return NotFound();

                var query = (from item in db.ПроектыИСотрудникиs
                             where item.КодРоли == id
                             select item).Any();

                if (query)
                    return RedirectToAction("DeletionForbidden");

                return View(tableItem);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteRole(Роли role)
        {
            using (var db = new CourseProject2DBContext())
            {
                if (ModelState.ErrorCount == 0)
                {
                    db.Ролиs.Remove(role);
                    db.SaveChanges();

                    return RedirectToAction("Index", new { tableName = "Роли" });
                }
            }

            return View(role);
        }

        [HttpGet]
        public IActionResult DeleteStatus(int id = -1)
        {
            if (id == -1)
                return NotFound();

            using (var db = new CourseProject2DBContext())
            {
                var tableItem = (from item in db.Статусыs
                                 where item.Код == id
                                 select item).FirstOrDefault();

                if (tableItem == null)
                    return NotFound();

                var query = (from item in db.ФактическиеТрудозатратыs
                             where item.КодСтатуса == id
                             select item).Any();

                if (query)
                    return RedirectToAction("DeletionForbidden");

                return View(tableItem);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteStatus(Статусы status)
        {
            using (var db = new CourseProject2DBContext())
            {
                if (ModelState.ErrorCount == 0)
                {
                    db.Статусыs.Remove(status);
                    db.SaveChanges();

                    return RedirectToAction("Index", new { tableName = "Статусы" });
                }
            }

            return View(status);
        }

        [HttpGet]
        public IActionResult DeleteProjectType(int id = -1)
        {
            if (id == -1)
                return NotFound();

            using (var db = new CourseProject2DBContext())
            {
                var tableItem = (from item in db.ТипыПроектовs
                                 where item.Код == id
                                 select item).FirstOrDefault();

                if (tableItem == null)
                    return NotFound();

                var query = (from item in db.Проектыs
                             where item.КодТипаПроекта == id
                             select item).Any();

                if (query)
                    return RedirectToAction("DeletionForbidden");

                return View(tableItem);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteProjectType(ТипыПроектов prType)
        {
            using (var db = new CourseProject2DBContext())
            {
                if (ModelState.ErrorCount == 0)
                {
                    db.ТипыПроектовs.Remove(prType);
                    db.SaveChanges();

                    return RedirectToAction("Index", new { tableName = "Типы проектов" });
                }
            }

            return View(prType);
        }
    }
}
