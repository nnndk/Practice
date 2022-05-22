using System;
using System.Collections.Generic;

namespace Practice.Models
{
    public partial class Пол
    {
        public Пол()
        {
            Сотрудникиs = new HashSet<Сотрудники>();
        }

        public int Код { get; set; }
        public string Пол1 { get; set; } = null!;

        public virtual ICollection<Сотрудники> Сотрудникиs { get; set; }
    }
}
