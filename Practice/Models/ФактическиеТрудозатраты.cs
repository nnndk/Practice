using System;
using System.Collections.Generic;

namespace Practice.Models
{
    public partial class ФактическиеТрудозатраты
    {
        public long Код { get; set; }
        public long КодРазработчика { get; set; }
        public int КодПроекта { get; set; }
        public string Задача { get; set; } = null!;
        public DateTime ДатаТрудозатраты { get; set; }
        public int КоличествоЧасов { get; set; }
        public int КодСтатуса { get; set; }
        public string? Комментарий { get; set; }
        public DateTime ПоследнееИзменение { get; set; }

        public virtual Проекты КодПроектаNavigation { get; set; } = null!;
        public virtual Сотрудники КодРазработчикаNavigation { get; set; } = null!;
        public virtual Статусы КодСтатусаNavigation { get; set; } = null!;
    }
}
