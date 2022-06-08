using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Practice.Models
{
    public partial class ПроектыИСотрудники
    {
        public long Код { get; set; }

        [Required(ErrorMessage = "Ошибка! Не выбран сотрудник!")]
        public long КодСотрудника { get; set; }

        [Required(ErrorMessage = "Ошибка! Не выбран проект")]
        public int КодПроекта { get; set; }

        [Required(ErrorMessage = "Ошибка! Не выбрана роль!")]
        public int КодРоли { get; set; }

        [Required(ErrorMessage = "Ошибка! Не указана дата начала работы на проекте!")]
        public DateTime ДатаНачалаРаботыНаПроекте { get; set; }
        public DateTime? ДатаОкончанияРаботыНаПроекте { get; set; }

        public virtual Проекты КодПроектаNavigation { get; set; } = null!;
        public virtual Роли КодРолиNavigation { get; set; } = null!;
        public virtual Сотрудники КодСотрудникаNavigation { get; set; } = null!;
    }
}
