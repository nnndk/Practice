using System;
using System.Collections.Generic;

namespace Practice.Models
{
    public partial class Сотрудники
    {
        public Сотрудники()
        {
            Департаментыs = new HashSet<Департаменты>();
            ДолжностиСотрудниковs = new HashSet<ДолжностиСотрудников>();
            Проектыs = new HashSet<Проекты>();
            ПроектыИСотрудникиs = new HashSet<ПроектыИСотрудники>();
            СтавкиСотрудниковs = new HashSet<СтавкиСотрудников>();
            УстройствоНаРаботуs = new HashSet<УстройствоНаРаботу>();
            ФактическиеТрудозатратыs = new HashSet<ФактическиеТрудозатраты>();
        }

        public long Код { get; set; }
        public string Фамилия { get; set; } = null!;
        public string Имя { get; set; } = null!;
        public string? Отчество { get; set; }
        public int КодПола { get; set; }
        public DateTime ДатаРождения { get; set; }
        public string Телефон1 { get; set; } = null!;
        public string? Телефон2 { get; set; }
        public DateTime? ДатаНачалаРаботыВSap { get; set; }
        public int КодВидаТрудоустройства { get; set; }
        public string Логин { get; set; } = null!;
        public string Пароль { get; set; } = null!;

        public virtual ВидыТрудоустройства КодВидаТрудоустройстваNavigation { get; set; } = null!;
        public virtual Пол КодПолаNavigation { get; set; } = null!;
        public virtual ICollection<Департаменты> Департаментыs { get; set; }
        public virtual ICollection<ДолжностиСотрудников> ДолжностиСотрудниковs { get; set; }
        public virtual ICollection<Проекты> Проектыs { get; set; }
        public virtual ICollection<ПроектыИСотрудники> ПроектыИСотрудникиs { get; set; }
        public virtual ICollection<СтавкиСотрудников> СтавкиСотрудниковs { get; set; }
        public virtual ICollection<УстройствоНаРаботу> УстройствоНаРаботуs { get; set; }
        public virtual ICollection<ФактическиеТрудозатраты> ФактическиеТрудозатратыs { get; set; }
    }
}
