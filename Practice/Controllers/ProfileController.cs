using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Practice.Data;
using Practice.Models;

namespace Practice.Controllers
{
    public class ProfileController : Controller
    {
        [Authorize]
        public IActionResult Index()
        {
            using (var db = new CourseProject2DBContext())
            {
                var query = (from emp in db.Сотрудникиs
                             join sex in db.Полs on emp.КодПола equals sex.Код
                             join empType in db.ВидыТрудоустройстваs on emp.КодВидаТрудоустройства equals empType.Код
                             where emp.Логин == User.Identity.Name
                             select new
                             {
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
                             }).First();

                var json_emp = JsonConvert.SerializeObject(query);
                var dict_emp = JsonConvert.DeserializeObject<Dictionary<string, string>>(json_emp);

                foreach (var key in dict_emp.Keys)
                {
                    if (dict_emp[key] == null || dict_emp[key] == "")
                        dict_emp[key] = "-";
                }

                return View(dict_emp);
            }
        }
    }
}
