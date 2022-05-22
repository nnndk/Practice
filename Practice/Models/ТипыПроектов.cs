using System;
using System.Collections.Generic;

namespace Practice.Models
{
    public partial class ТипыПроектов
    {
        public ТипыПроектов()
        {
            Проектыs = new HashSet<Проекты>();
        }

        public int Код { get; set; }
        public string ТипПроекта { get; set; } = null!;

        public virtual ICollection<Проекты> Проектыs { get; set; }
    }
}
