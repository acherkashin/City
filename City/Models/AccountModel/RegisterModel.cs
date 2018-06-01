using System.ComponentModel.DataAnnotations;

namespace CyberCity.Models.AccountModel
{
    public class RegisterModel
    {
        [Required(ErrorMessage = "Не указана фамилия")]
        public string LastName { get; set; }

        [Required(ErrorMessage = "Не указано имя")]
        public string FirstName { get; set; }

        [Required(ErrorMessage = "Не указан логин")]
        public string Login { get; set; }

        public Subject Subject { get; set; }

        [DataType(DataType.Url)]
        public string ArduinoUrl { get; set; }

        [Required(ErrorMessage = "Не указан пароль")]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Compare("Password", ErrorMessage = "Пароль введен неверно")]
        public string ConfirmPassword { get; set; }

        public User ToUser()
        {
            return new User
            {
                Login = Login,
                Password = Password,
                FirstName = FirstName,
                LastName = LastName,
                Subject = Subject,
                ArduinoUrl = ArduinoUrl
            };
        }
    }
}
