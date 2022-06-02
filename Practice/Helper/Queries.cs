using Practice.Data;

namespace Practice.Helper
{
    public static class Queries
    {
        private static readonly Dictionary<string, string> dictRolesPosViews = new Dictionary<string, string>() {
            { "Разработчик", "Developer" },
            { "Менеджер проекта", "ProjectManager" },
            { "Администратор", "Admin" },
            { "Администратор баз данных", "DbAdmin" },
            { "Директор департамента", "DepDirector" },
            { "Генеральный директор", "CEO" },
            { "Бухгалтер", "Accountant" },
            { "Сотрудник договорного отдела", "ContractDepEmp" }
        };

        public static readonly string[] developerTypes = new string[] { "Разработчик 1 категории", "Разработчик 2 категории",
                    "Разработчик 3 категории", "Разработчик 4 категории", "Разработчик 5 категории", "Разработчик-эксперт" };

        public static Dictionary<string, string> GetEmployeeRoles(string login)
        {
            // Получает все роли сотрудника (нужно для авторизации и отображения необходимых вкладок)
            using (var db = new CourseProject2DBContext())
            {
                // должности
                var query = from pos in db.Должностиs
                            join empPos in db.ДолжностиСотрудниковs on pos.Код equals empPos.КодДолжности
                            join emp in db.Сотрудникиs on empPos.КодСотрудника equals emp.Код
                            where emp.Логин == login
                            select new
                            {
                                Должность = pos.Должность
                            };

                var empPositions = new Dictionary<string, string>();

                foreach (var pos in query)
                {
                    if (developerTypes.Contains(pos.Должность))
                    {
                        if (!empPositions.Keys.Contains("Разработчик"))
                            empPositions.Add("Разработчик", dictRolesPosViews["Разработчик"]);
                    }
                    else if (!empPositions.Keys.Contains(pos.Должность))
                        empPositions.Add(pos.Должность, dictRolesPosViews[pos.Должность]);
                }

                // роль менеджер проекта
                int query1 = (from project in db.Проектыs
                              join emp in db.Сотрудникиs on project.КодМенеджераПроекта equals emp.Код
                              where emp.Логин == login && project.ДатаЗавершенияПроекта == null
                              select project.Код).Count();

                if (query1 > 0)
                    empPositions.Add("Менеджер проекта", dictRolesPosViews["Менеджер проекта"]);

                if (!empPositions.ContainsKey("Разработчик"))
                {
                    // роли разработчик и удалённый разработчик
                    var query2 = (from pr_emps in db.ПроектыИСотрудникиs
                                  join role in db.Ролиs on pr_emps.КодРоли equals role.Код
                                  join project in db.Проектыs on pr_emps.КодПроекта equals project.Код
                                  join emp in db.Сотрудникиs on pr_emps.КодСотрудника equals emp.Код
                                  where emp.Логин == login && project.ДатаЗавершенияПроекта == null
                                  select role.Роль).Count();

                    if (query2 > 0)
                        empPositions.Add("Разработчик", dictRolesPosViews["Разработчик"]);
                }

                return empPositions;
            }
        }
    }
}
