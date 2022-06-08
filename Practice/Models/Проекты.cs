using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Practice.Models
{
    public partial class Проекты
    {
        public Проекты()
        {
            ПроектыИСотрудникиs = new HashSet<ПроектыИСотрудники>();
            СтавкиСотрудниковs = new HashSet<СтавкиСотрудников>();
            ФактическиеТрудозатратыs = new HashSet<ФактическиеТрудозатраты>();
        }

        public int Код { get; set; }

        [Required(ErrorMessage = "Ошибка! Не указано название проекта!")]
        public string НазваниеПроекта { get; set; } = null!;

        [Required(ErrorMessage = "Ошибка! Не указана дата начала проекта!")]
        public DateTime ДатаНачалаПроекта { get; set; }

        public DateTime? ДатаЗавершенияПроекта { get; set; }

        [Required(ErrorMessage = "Ошибка! Не выбран менеджер проекта!")]
        public long КодМенеджераПроекта { get; set; }

        [Required(ErrorMessage = "Ошибка! Не выбран тип проекта!")]
        public int КодТипаПроекта { get; set; }

        public virtual Сотрудники КодМенеджераПроектаNavigation { get; set; } = null!;
        public virtual ТипыПроектов КодТипаПроектаNavigation { get; set; } = null!;
        public virtual ICollection<ПроектыИСотрудники> ПроектыИСотрудникиs { get; set; }
        public virtual ICollection<СтавкиСотрудников> СтавкиСотрудниковs { get; set; }
        public virtual ICollection<ФактическиеТрудозатраты> ФактическиеТрудозатратыs { get; set; }
    }
}
