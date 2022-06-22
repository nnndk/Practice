using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Practice.Models
{
    public partial class ФактическиеТрудозатраты
    {
        [Required(ErrorMessage = "Ошибка! Не выбрана трудозатрата!")]
        public long? Код { get; set; }

        [Required(ErrorMessage = "Ошибка! Не выбран разработчик!")]
        public long? КодРазработчика { get; set; }

        [Required(ErrorMessage = "Ошибка! Не выбран проект!")]
        public int? КодПроекта { get; set; }

        [Required(ErrorMessage = "Ошибка! Не указана задача!")]
        public string? Задача { get; set; } = null!;

        [Required(ErrorMessage = "Ошибка! Не указана дата трудозатраты!")]
        public DateTime? ДатаТрудозатраты { get; set; }

        [Required(ErrorMessage = "Ошибка! Не указано количество часов!")]
        public int? КоличествоЧасов { get; set; }
        public int КодСтатуса { get; set; }
        public string? Комментарий { get; set; }
        public DateTime ПоследнееИзменение { get; set; }

        public virtual Проекты КодПроектаNavigation { get; set; } = null!;
        public virtual Сотрудники КодРазработчикаNavigation { get; set; } = null!;
        public virtual Статусы КодСтатусаNavigation { get; set; } = null!;
    }
}
