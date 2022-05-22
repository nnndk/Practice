using System;
using System.Collections.Generic;

namespace Practice.Models
{
    public partial class Статусы
    {
        public Статусы()
        {
            ФактическиеТрудозатратыs = new HashSet<ФактическиеТрудозатраты>();
        }

        public int Код { get; set; }
        public string Статус { get; set; } = null!;

        public virtual ICollection<ФактическиеТрудозатраты> ФактическиеТрудозатратыs { get; set; }
    }
}
