using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Practice.Data;
using Practice.Helper;
using Practice.Models.ViewModels;

namespace Practice.Controllers
{
    [Authorize(Roles = "Бухгалтер")]
    public class AccountantController : Controller
    {
        private static string[] reportTypes = new string[] { "Отчёт по выручке" };
        private static IActionResult? report = null;

        [HttpGet]
        public IActionResult Index(ReportViewModel? page = null)
        {
            if (page == null)
                return View(new ReportViewModel());

            ViewBag.ReportTypes = reportTypes;

            return View(page);
        }

        [HttpPost, ActionName("Index")]
        [ValidateAntiForgeryToken]
        public IActionResult IndexPOST(ReportViewModel page, bool printReport = false)
        {
            if (report != null && printReport)
                return report;

            if (!reportTypes.Contains(page.ReportType))
                ModelState.AddModelError("ReportType", "Ошибка! Необходимо выбрать тип отчёта!");

            if (page.StartDate > page.EndDate)
                ModelState.AddModelError("EndDate", "Ошибка! Дата окончания периода должна быть больше даты его начала!");

            if (ModelState.ErrorCount == 0)
            {
                using (var db = new CourseProject2DBContext())
                {
                    switch (page.ReportType)
                    {
                        case "Отчёт по выручке":
                            page.Report = new List<Dictionary<string, string>>();

                            if (page.DepId == null)
                            {
                                if (page.OnlyCurrEmployees)
                                {
                                    var allRates = (from task in db.ФактическиеТрудозатратыs
                                                    join project in db.Проектыs on task.КодПроекта equals project.Код
                                                    join projectType in db.ТипыПроектовs on project.КодТипаПроекта equals projectType.Код
                                                    join status in db.Статусыs on task.КодСтатуса equals status.Код
                                                    join rate in db.СтавкиСотрудниковs on new { x1 = task.КодПроекта, x2 = task.КодРазработчика }
                                                    equals new { x1 = rate.КодПроекта, x2 = rate.КодСотрудника } into gr
                                                    from g in gr.DefaultIfEmpty()
                                                    where projectType.ТипПроекта == "Внешний" && task.ДатаТрудозатраты >= page.StartDate
                                                    && task.ДатаТрудозатраты <= page.EndDate && status.Статус == "Подтверждено"
                                                    select new
                                                    {
                                                        task.Код,
                                                        task.КодРазработчика,
                                                        task.КоличествоЧасов,
                                                        task.ДатаТрудозатраты,
                                                        ДатаНачалаДействияСтавки = g.ДатаНачалаДействияСтавки.Value.ToString("yyyy-MM-dd") ?? new DateTime(2000, 1, 1).ToString("yyyy-MM-dd"),
                                                        Ставка = int.Parse(g.Ставка.ToString() ?? "0")
                                                    }).Distinct().ToList();

                                    var lessRates = from task in allRates
                                                    where DateTime.Parse(task.ДатаНачалаДействияСтавки) <= task.ДатаТрудозатраты
                                                    select task;

                                    var currRates = from task in lessRates
                                                    group task by task.Код into g
                                                    select new
                                                    {
                                                        g.Key,
                                                        КодРазработчика = g.Select(x => x.КодРазработчика).First(),
                                                        КоличествоЧасов = g.Select(x => x.КоличествоЧасов).First(),
                                                        ДатаНачалаДействияСтавки = g.Max(x => x.ДатаНачалаДействияСтавки)
                                                    };

                                    var profitByTask = from task in currRates
                                                       join rate in lessRates on new { Код = task.Key, task.ДатаНачалаДействияСтавки } equals new { rate.Код, rate.ДатаНачалаДействияСтавки }
                                                       select new
                                                       {
                                                           rate.Код,
                                                           task.КодРазработчика,
                                                           Выручка = task.КоличествоЧасов * rate.Ставка
                                                       };

                                    var profitByEmp = from item in profitByTask
                                                      group item by item.КодРазработчика into g
                                                      select new
                                                      {
                                                          Код = g.Key,
                                                          Выручка = g.Sum(x => x.Выручка)
                                                      };

                                    var queryResult = (from emp in db.Сотрудникиs.ToList()
                                                       join empType in db.ВидыТрудоустройстваs on emp.КодВидаТрудоустройства equals empType.Код
                                                       join dev in profitByEmp on emp.Код equals dev.Код into gr
                                                       from g in gr.DefaultIfEmpty()
                                                       where (from empl in db.УстройствоНаРаботуs
                                                              where empl.ДатаУвольнения > DateTime.Now.Date
                                                              || empl.ДатаУвольнения == null
                                                              select empl.КодСотрудника).ToList().Contains(emp.Код)
                                                       select new
                                                       {
                                                           emp.Код,
                                                           emp.Фамилия,
                                                           emp.Имя,
                                                           emp.Отчество,
                                                           emp.Логин,
                                                           empType.ВидТрудоустройства,
                                                           Выручка = (g == null ? 0 : g.Выручка)
                                                       }).ToList();

                                    foreach (var item in queryResult)
                                    {
                                        var json = JsonConvert.SerializeObject(item);
                                        page.Report.Add(JsonConvert.DeserializeObject<Dictionary<string, string>>(json));
                                    }
                                }
                                else
                                {
                                    var allRates = (from task in db.ФактическиеТрудозатратыs
                                                    join project in db.Проектыs on task.КодПроекта equals project.Код
                                                    join projectType in db.ТипыПроектовs on project.КодТипаПроекта equals projectType.Код
                                                    join status in db.Статусыs on task.КодСтатуса equals status.Код
                                                    join rate in db.СтавкиСотрудниковs
                                                    on new { x1 = task.КодПроекта, x2 = task.КодРазработчика } equals new { x1 = rate.КодПроекта, x2 = rate.КодСотрудника } into gr
                                                    from g in gr.DefaultIfEmpty()
                                                    where projectType.ТипПроекта == "Внешний" && task.ДатаТрудозатраты >= page.StartDate 
                                                    && task.ДатаТрудозатраты <= page.EndDate && status.Статус == "Подтверждено"
                                                    select new
                                                    {
                                                        task.Код,
                                                        task.КодРазработчика,
                                                        task.КоличествоЧасов,
                                                        task.ДатаТрудозатраты,
                                                        ДатаНачалаДействияСтавки = g.ДатаНачалаДействияСтавки.Value.ToString("yyyy-MM-dd") ?? new DateTime(2000, 1, 1).ToString("yyyy-MM-dd"),
                                                        Ставка = int.Parse(g.Ставка.ToString() ?? "0")
                                                    }).Distinct().ToList();

                                    var lessRates = from task in allRates
                                                    where DateTime.Parse(task.ДатаНачалаДействияСтавки) <= task.ДатаТрудозатраты
                                                    select task;

                                    var currRates = from task in lessRates
                                                    group task by task.Код into g
                                                    select new
                                                    {
                                                        g.Key,
                                                        КодРазработчика = g.Select(x => x.КодРазработчика).First(),
                                                        КоличествоЧасов = g.Select(x => x.КоличествоЧасов).First(),
                                                        ДатаНачалаДействияСтавки = g.Max(x => x.ДатаНачалаДействияСтавки)
                                                    };

                                    var profitByTask = from task in currRates
                                                       join rate in lessRates on new { Код = task.Key, task.ДатаНачалаДействияСтавки } equals new { rate.Код, rate.ДатаНачалаДействияСтавки }
                                                       select new
                                                       {
                                                           rate.Код,
                                                           task.КодРазработчика,
                                                           Выручка = task.КоличествоЧасов * rate.Ставка
                                                       };

                                    var profitByEmp = from item in profitByTask
                                                      group item by item.КодРазработчика into g
                                                      select new
                                                      {
                                                          Код = g.Key,
                                                          Выручка = g.Sum(x => x.Выручка)
                                                      };

                                    var queryResult = (from emp in db.Сотрудникиs.ToList()
                                                       join empType in db.ВидыТрудоустройстваs on emp.КодВидаТрудоустройства equals empType.Код
                                                       join dev in profitByEmp on emp.Код equals dev.Код into gr
                                                       from g in gr.DefaultIfEmpty()
                                                       select new
                                                       {
                                                           emp.Код,
                                                           emp.Фамилия,
                                                           emp.Имя,
                                                           emp.Отчество,
                                                           emp.Логин,
                                                           empType.ВидТрудоустройства,
                                                           Выручка = (g == null ? 0 : g.Выручка)
                                                       }).ToList();

                                    foreach (var item in queryResult)
                                    {
                                        var json = JsonConvert.SerializeObject(item);
                                        page.Report.Add(JsonConvert.DeserializeObject<Dictionary<string, string>>(json));
                                    }
                                }
                            }
                            else
                            {
                                if (page.OnlyCurrEmployees)
                                {
                                    var allRates = (from task in db.ФактическиеТрудозатратыs
                                                    join project in db.Проектыs on task.КодПроекта equals project.Код
                                                    join projectType in db.ТипыПроектовs on project.КодТипаПроекта equals projectType.Код
                                                    join status in db.Статусыs on task.КодСтатуса equals status.Код
                                                    join rate in db.СтавкиСотрудниковs on new { x1 = task.КодПроекта, x2 = task.КодРазработчика }
                                                    equals new { x1 = rate.КодПроекта, x2 = rate.КодСотрудника } into gr
                                                    from g in gr.DefaultIfEmpty()
                                                    where projectType.ТипПроекта == "Внешний" && task.ДатаТрудозатраты >= page.StartDate
                                                    && task.ДатаТрудозатраты <= page.EndDate && status.Статус == "Подтверждено"
                                                    select new
                                                    {
                                                        task.Код,
                                                        task.КодРазработчика,
                                                        task.КоличествоЧасов,
                                                        task.ДатаТрудозатраты,
                                                        ДатаНачалаДействияСтавки = g.ДатаНачалаДействияСтавки.Value.ToString("yyyy-MM-dd") ?? new DateTime(2000, 1, 1).ToString("yyyy-MM-dd"),
                                                        Ставка = int.Parse(g.Ставка.ToString() ?? "0")
                                                    }).Distinct().ToList();

                                    var lessRates = from task in allRates
                                                    where DateTime.Parse(task.ДатаНачалаДействияСтавки) <= task.ДатаТрудозатраты
                                                    select task;

                                    var currRates = from task in lessRates
                                                    group task by task.Код into g
                                                    select new
                                                    {
                                                        g.Key,
                                                        КодРазработчика = g.Select(x => x.КодРазработчика).First(),
                                                        КоличествоЧасов = g.Select(x => x.КоличествоЧасов).First(),
                                                        ДатаНачалаДействияСтавки = g.Max(x => x.ДатаНачалаДействияСтавки)
                                                    };

                                    var profitByTask = from task in currRates
                                                       join rate in lessRates on new { Код = task.Key, task.ДатаНачалаДействияСтавки } equals new { rate.Код, rate.ДатаНачалаДействияСтавки }
                                                       select new
                                                       {
                                                           rate.Код,
                                                           task.КодРазработчика,
                                                           Выручка = task.КоличествоЧасов * rate.Ставка
                                                       };

                                    var profitByEmp = from item in profitByTask
                                                      group item by item.КодРазработчика into g
                                                      select new
                                                      {
                                                          Код = g.Key,
                                                          Выручка = g.Sum(x => x.Выручка)
                                                      };

                                    var queryResult = (from emp in db.Сотрудникиs.ToList()
                                                       join empType in db.ВидыТрудоустройстваs on emp.КодВидаТрудоустройства equals empType.Код
                                                       join dev in profitByEmp on emp.Код equals dev.Код into gr
                                                       from g in gr.DefaultIfEmpty()
                                                       where (from empl in db.УстройствоНаРаботуs
                                                              where empl.ДатаУвольнения > DateTime.Now.Date
                                                              || empl.ДатаУвольнения == null
                                                              select empl.КодСотрудника).ToList().Contains(emp.Код)
                                                                 && (from empPos in db.ДолжностиСотрудниковs
                                                                     where empPos.КодДепартамента == page.DepId
                                                                     select empPos.КодСотрудника).Distinct().ToList()
                                                                                                      .Contains(emp.Код)
                                                       select new
                                                       {
                                                           emp.Код,
                                                           emp.Фамилия,
                                                           emp.Имя,
                                                           emp.Отчество,
                                                           emp.Логин,
                                                           empType.ВидТрудоустройства,
                                                           Выручка = (g == null ? 0 : g.Выручка)
                                                       }).ToList();

                                    foreach (var item in queryResult)
                                    {
                                        var json = JsonConvert.SerializeObject(item);
                                        page.Report.Add(JsonConvert.DeserializeObject<Dictionary<string, string>>(json));
                                    }
                                }
                                else
                                {
                                    var allRates = (from task in db.ФактическиеТрудозатратыs
                                                    join project in db.Проектыs on task.КодПроекта equals project.Код
                                                    join projectType in db.ТипыПроектовs on project.КодТипаПроекта equals projectType.Код
                                                    join status in db.Статусыs on task.КодСтатуса equals status.Код
                                                    join rate in db.СтавкиСотрудниковs on new { x1 = task.КодПроекта, x2 = task.КодРазработчика }
                                                    equals new { x1 = rate.КодПроекта, x2 = rate.КодСотрудника } into gr
                                                    from g in gr.DefaultIfEmpty()
                                                    where projectType.ТипПроекта == "Внешний" && task.ДатаТрудозатраты >= page.StartDate
                                                    && task.ДатаТрудозатраты <= page.EndDate && status.Статус == "Подтверждено"
                                                    select new
                                                    {
                                                        task.Код,
                                                        task.КодРазработчика,
                                                        task.КоличествоЧасов,
                                                        task.ДатаТрудозатраты,
                                                        ДатаНачалаДействияСтавки = g.ДатаНачалаДействияСтавки.Value.ToString("yyyy-MM-dd") ?? new DateTime(2000, 1, 1).ToString("yyyy-MM-dd"),
                                                        Ставка = int.Parse(g.Ставка.ToString() ?? "0")
                                                    }).Distinct().ToList();

                                    var lessRates = from task in allRates
                                                    where DateTime.Parse(task.ДатаНачалаДействияСтавки) <= task.ДатаТрудозатраты
                                                    select task;

                                    var currRates = from task in lessRates
                                                    group task by task.Код into g
                                                    select new
                                                    {
                                                        g.Key,
                                                        КодРазработчика = g.Select(x => x.КодРазработчика).First(),
                                                        КоличествоЧасов = g.Select(x => x.КоличествоЧасов).First(),
                                                        ДатаНачалаДействияСтавки = g.Max(x => x.ДатаНачалаДействияСтавки)
                                                    };

                                    var profitByTask = from task in currRates
                                                       join rate in lessRates on new { Код = task.Key, task.ДатаНачалаДействияСтавки } equals new { rate.Код, rate.ДатаНачалаДействияСтавки }
                                                       select new
                                                       {
                                                           rate.Код,
                                                           task.КодРазработчика,
                                                           Выручка = task.КоличествоЧасов * rate.Ставка
                                                       };

                                    var profitByEmp = from item in profitByTask
                                                      group item by item.КодРазработчика into g
                                                      select new
                                                      {
                                                          Код = g.Key,
                                                          Выручка = g.Sum(x => x.Выручка)
                                                      };

                                    var queryResult = (from emp in db.Сотрудникиs.ToList()
                                                       join empType in db.ВидыТрудоустройстваs on emp.КодВидаТрудоустройства equals empType.Код
                                                       join dev in profitByEmp on emp.Код equals dev.Код into gr
                                                       from g in gr.DefaultIfEmpty()
                                                       where (from empPos in db.ДолжностиСотрудниковs
                                                              where empPos.КодДепартамента == page.DepId
                                                              select empPos.КодСотрудника).Distinct().ToList()
                                                                                                      .Contains(emp.Код)
                                                       select new
                                                       {
                                                           emp.Код,
                                                           emp.Фамилия,
                                                           emp.Имя,
                                                           emp.Отчество,
                                                           emp.Логин,
                                                           empType.ВидТрудоустройства,
                                                           Выручка = (g == null ? 0 : g.Выручка)
                                                       }).ToList();

                                    foreach (var item in queryResult)
                                    {
                                        var json = JsonConvert.SerializeObject(item);
                                        page.Report.Add(JsonConvert.DeserializeObject<Dictionary<string, string>>(json));
                                    }
                                }
                            }

                            if (page.Report.Count == 0)
                            {
                                var list = new List<Dictionary<string, string>>();
                                var dict = new Dictionary<string, string>();
                                var keys = new string[] { "Код", "Фамилия", "Имя", "Отчество", "Логин", "ВидТрудоустройства", "Выручка" };

                                foreach (var key in keys)
                                    dict.Add(key, null);

                                list.Add(dict);

                                page.Report = list;
                            }

                            break;
                        default:
                            page.Report = null;
                            ModelState.AddModelError("ReportType", "Ошибка! Необходимо выбрать тип отчёта!");
                            break;
                    }
                }

                report = CommonFunctions.GetReport(page);
            }

            ViewBag.ReportTypes = reportTypes;

            return View(page);
        }
    }
}
