using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Practice.Models
{
    public partial class Департаменты
    {
        public Департаменты()
        {
            ДолжностиСотрудниковs = new HashSet<ДолжностиСотрудников>();
        }

        [Required(ErrorMessage = "Ошибка! Не указан код департамента!")]
        public int Код { get; set; }

        [Required(ErrorMessage = "Ошибка! Не указано название департамента!")]
        public string НазваниеДепартамента { get; set; } = null!;

        [Required(ErrorMessage = "Ошибка! Не указан код директора департамента!")]
        public long КодДиректораДепартамента { get; set; }

        public virtual Сотрудники КодДиректораДепартаментаNavigation { get; set; } = null!;
        public virtual ICollection<ДолжностиСотрудников> ДолжностиСотрудниковs { get; set; }
    }
}
