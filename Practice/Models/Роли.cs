using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Practice.Models
{
    public partial class Роли
    {
        public Роли()
        {
            ПроектыИСотрудникиs = new HashSet<ПроектыИСотрудники>();
        }

        [Required(ErrorMessage = "Ошибка! Не выбрана роль!")]
        public int? Код { get; set; }

        [Required(ErrorMessage = "Ошибка! Не указано название роли!")]
        public string? Роль { get; set; } = null!;

        public virtual ICollection<ПроектыИСотрудники> ПроектыИСотрудникиs { get; set; }
    }
}
