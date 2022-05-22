using System;
using System.Collections.Generic;

namespace Practice.Models
{
    public partial class Должности
    {
        public Должности()
        {
            ДолжностиСотрудниковs = new HashSet<ДолжностиСотрудников>();
        }

        public int Код { get; set; }
        public string Должность { get; set; } = null!;

        public virtual ICollection<ДолжностиСотрудников> ДолжностиСотрудниковs { get; set; }
    }
}
