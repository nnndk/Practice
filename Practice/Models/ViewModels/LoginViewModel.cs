using System.ComponentModel.DataAnnotations;

namespace Practice.Models.ViewModels
{
    public class LoginViewModel
    {
        [Required(ErrorMessage = "Ошибка! Не указан логин!")]
        public string Login { get; set; }

        [Required(ErrorMessage = "Ошибка! Не указан пароль!")]
        [DataType(DataType.Password)]
        public string Password { get; set; }
    }
}
