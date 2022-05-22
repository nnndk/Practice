using System;
using System.Collections.Generic;

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
        public string НазваниеПроекта { get; set; } = null!;
        public DateTime ДатаНачалаПроекта { get; set; }
        public DateTime? ДатаЗавершенияПроекта { get; set; }
        public long КодМенеджераПроекта { get; set; }
        public int КодТипаПроекта { get; set; }

        public virtual Сотрудники КодМенеджераПроектаNavigation { get; set; } = null!;
        public virtual ТипыПроектов КодТипаПроектаNavigation { get; set; } = null!;
        public virtual ICollection<ПроектыИСотрудники> ПроектыИСотрудникиs { get; set; }
        public virtual ICollection<СтавкиСотрудников> СтавкиСотрудниковs { get; set; }
        public virtual ICollection<ФактическиеТрудозатраты> ФактическиеТрудозатратыs { get; set; }
    }
}
