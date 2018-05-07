using System.ComponentModel.DataAnnotations;

namespace City.Models
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
        NuclearPowerPlant,

        [Display(Name = "Электрическая подстанция")]
        ElectricalSubstation,

        [Display(Name = "Метестанция")]
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
