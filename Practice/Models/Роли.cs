using System;
using System.Collections.Generic;

namespace Practice.Models
{
    public partial class Роли
    {
        public Роли()
        {
            ПроектыИСотрудникиs = new HashSet<ПроектыИСотрудники>();
        }

        public int Код { get; set; }
        public string Роль { get; set; } = null!;

        public virtual ICollection<ПроектыИСотрудники> ПроектыИСотрудникиs { get; set; }
    }
}
