using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Practice.Models
{
    public partial class Должности
    {
        public Должности()
        {
            ДолжностиСотрудниковs = new HashSet<ДолжностиСотрудников>();
        }

        [Required(ErrorMessage = "Ошибка! Не выбрана должность!")]
        public int Код { get; set; }

        [Required(ErrorMessage = "Ошибка! Не указано название должности!")]
        public string Должность { get; set; } = null!;

        public virtual ICollection<ДолжностиСотрудников> ДолжностиСотрудниковs { get; set; }
    }
}
