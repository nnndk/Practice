using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Practice.Models
{
    public partial class ДолжностиСотрудников
    {
        public long Код { get; set; }

        [Required(ErrorMessage = "Ошибка! Не выбран департамент!")]
        public int КодДепартамента { get; set; }

        [Required(ErrorMessage = "Ошибка! Не указан код сотрудника!")]
        public long КодСотрудника { get; set; }

        [Required(ErrorMessage = "Ошибка! Не указана дата назначения сотрудника!")]
        public DateTime ДатаНазначения { get; set; }

        [Required(ErrorMessage = "Ошибка! Не выбрана должность!")]
        public int КодДолжности { get; set; }

        public virtual Департаменты КодДепартаментаNavigation { get; set; } = null!;
        public virtual Должности КодДолжностиNavigation { get; set; } = null!;
        public virtual Сотрудники КодСотрудникаNavigation { get; set; } = null!;
    }
}
