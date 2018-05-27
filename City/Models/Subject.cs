using System.ComponentModel.DataAnnotations;

namespace CyberCity.Models
{
    public enum Subject
    {
        [Display(Name = "Администратор")]
        Admin,

        [Display(Name = "Почтамп")]
        PostOffice,

        [Display(Name = "Муниципалитет")]
        Municipality,

        [Display(Name = "Банк")]
        Bank,

        [Display(Name = "Аэропорт")]
        Airport,

        [Display(Name = "Атомная станция")]
        NuclearStation,

        [Display(Name = "Подстанция")]
        Substation,

        [Display(Name = "Метеостанция")]
        WeatherStation,

        [Display(Name = "Жилые дома")]
        Houses,

        [Display(Name = "Завод")]
        Factory,

        [Display(Name = "Дорожная служба")]
        RoadService,

        [Display(Name = "Хакер")]
        Hacker,
    }
}
