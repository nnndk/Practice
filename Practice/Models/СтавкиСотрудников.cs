using System;
using System.Collections.Generic;

namespace Practice.Models
{
    public partial class СтавкиСотрудников
    {
        public long Код { get; set; }
        public long КодСотрудника { get; set; }
        public int КодПроекта { get; set; }
        public DateTime ДатаНачалаДействияСтавки { get; set; }
        public long Ставка { get; set; }

        public virtual Проекты КодПроектаNavigation { get; set; } = null!;
        public virtual Сотрудники КодСотрудникаNavigation { get; set; } = null!;
    }
}
