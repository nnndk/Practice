using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Practice.Models
{
    public partial class ТипыПроектов
    {
        public ТипыПроектов()
        {
            Проектыs = new HashSet<Проекты>();
        }

        [Required(ErrorMessage = "Ошибка! Не выбран тип проекта!")]
        public int Код { get; set; }

        [Required(ErrorMessage = "Ошибка! Не указано название типа проекта!")]
        public string ТипПроекта { get; set; } = null!;

        public virtual ICollection<Проекты> Проектыs { get; set; }
    }
}
