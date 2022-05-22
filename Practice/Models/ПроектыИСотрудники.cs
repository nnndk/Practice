using System;
using System.Collections.Generic;

namespace Practice.Models
{
    public partial class ПроектыИСотрудники
    {
        public long Код { get; set; }
        public long КодСотрудника { get; set; }
        public int КодПроекта { get; set; }
        public int КодРоли { get; set; }
        public DateTime ДатаНачалаРаботыНаПроекте { get; set; }
        public DateTime? ДатаОкончанияРаботыНаПроекте { get; set; }

        public virtual Проекты КодПроектаNavigation { get; set; } = null!;
        public virtual Роли КодРолиNavigation { get; set; } = null!;
        public virtual Сотрудники КодСотрудникаNavigation { get; set; } = null!;
    }
}
