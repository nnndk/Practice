using System;
using System.Collections.Generic;

namespace Practice.Models
{
    public partial class УстройствоНаРаботу
    {
        public long КодУстройстваНаРаботу { get; set; }
        public long КодСотрудника { get; set; }
        public DateTime ДатаЗачисленияВШтат { get; set; }
        public DateTime? ДатаУвольнения { get; set; }

        public virtual Сотрудники КодСотрудникаNavigation { get; set; } = null!;
    }
}
