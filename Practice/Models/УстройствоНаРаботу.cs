using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Practice.Models
{
    public partial class УстройствоНаРаботу
    {
        public long КодУстройстваНаРаботу { get; set; }

        [Required(ErrorMessage = "Ошибка! Не выбран сотрудник!")]
        public long КодСотрудника { get; set; }

        [Required(ErrorMessage = "Ошибка! Не указана дата зачисления в штат!")]
        public DateTime ДатаЗачисленияВШтат { get; set; }

        public DateTime? ДатаУвольнения { get; set; }

        public virtual Сотрудники КодСотрудникаNavigation { get; set; } = null!;
    }
}
