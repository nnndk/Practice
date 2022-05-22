using System;
using System.Collections.Generic;

namespace Practice.Models
{
    public partial class ВидыТрудоустройства
    {
        public ВидыТрудоустройства()
        {
            Сотрудникиs = new HashSet<Сотрудники>();
        }

        public int Код { get; set; }
        public string ВидТрудоустройства { get; set; } = null!;

        public virtual ICollection<Сотрудники> Сотрудникиs { get; set; }
    }
}
