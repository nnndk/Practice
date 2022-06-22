using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Practice.Models
{
    public partial class Статусы
    {
        public Статусы()
        {
            ФактическиеТрудозатратыs = new HashSet<ФактическиеТрудозатраты>();
        }

        [Required(ErrorMessage = "Ошибка! Не выбран статус!")]
        public int? Код { get; set; }

        [Required(ErrorMessage = "Ошибка! Не указано название статуса!")]
        public string? Статус { get; set; } = null!;

        public virtual ICollection<ФактическиеТрудозатраты> ФактическиеТрудозатратыs { get; set; }
    }
}
