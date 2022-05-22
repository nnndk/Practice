using System;
using System.Collections.Generic;

namespace Practice.Models
{
    public partial class ДолжностиСотрудников
    {
        public long Код { get; set; }
        public int КодДепартамента { get; set; }
        public long КодСотрудника { get; set; }
        public DateTime ДатаНазначения { get; set; }
        public int КодДолжности { get; set; }

        public virtual Департаменты КодДепартаментаNavigation { get; set; } = null!;
        public virtual Должности КодДолжностиNavigation { get; set; } = null!;
        public virtual Сотрудники КодСотрудникаNavigation { get; set; } = null!;
    }
}
