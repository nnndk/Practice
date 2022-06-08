using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Practice.Models
{
    public partial class ВидыТрудоустройства
    {
        public ВидыТрудоустройства()
        {
            Сотрудникиs = new HashSet<Сотрудники>();
        }

        [Required(ErrorMessage = "Ошибка! Не выбран вид трудоустройства!")]
        public int Код { get; set; }

        [Required(ErrorMessage = "Ошибка! Не указан вид трудоустройства!")]
        public string ВидТрудоустройства { get; set; } = null!;

        public virtual ICollection<Сотрудники> Сотрудникиs { get; set; }
    }
}
