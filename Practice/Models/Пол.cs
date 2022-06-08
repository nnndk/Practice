using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Practice.Models
{
    public partial class Пол
    {
        public Пол()
        {
            Сотрудникиs = new HashSet<Сотрудники>();
        }

        public int Код { get; set; }

        [Required(ErrorMessage = "Ошибка! Не указано название пола!")]
        public string Пол1 { get; set; } = null!;

        public virtual ICollection<Сотрудники> Сотрудникиs { get; set; }
    }
}
