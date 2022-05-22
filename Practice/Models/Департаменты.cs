using System;
using System.Collections.Generic;

namespace Practice.Models
{
    public partial class Департаменты
    {
        public Департаменты()
        {
            ДолжностиСотрудниковs = new HashSet<ДолжностиСотрудников>();
        }

        public int Код { get; set; }
        public string НазваниеДепартамента { get; set; } = null!;
        public long КодДиректораДепартамента { get; set; }

        public virtual Сотрудники КодДиректораДепартаментаNavigation { get; set; } = null!;
        public virtual ICollection<ДолжностиСотрудников> ДолжностиСотрудниковs { get; set; }
    }
}
