using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Practice.Models
{
    public partial class СтавкиСотрудников
    {
        public long Код { get; set; }

        [Required(ErrorMessage = "Ошибка! Не выбран сотрудник!")]
        public long? КодСотрудника { get; set; }

        [Required(ErrorMessage = "Ошибка! Не выбран проект!")]
        public int? КодПроекта { get; set; }

        [Required(ErrorMessage = "Ошибка! Не указана дата начала действия ставки!")]
        public DateTime? ДатаНачалаДействияСтавки { get; set; }

        [Required(ErrorMessage = "Ошибка! Не указана ставка!")]
        public long? Ставка { get; set; }

        public virtual Проекты КодПроектаNavigation { get; set; } = null!;
        public virtual Сотрудники КодСотрудникаNavigation { get; set; } = null!;
    }
}
