using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace CyberCity.Models.AirportModels
{
    public enum FlightStates
    {
        [Display(Name = "Улетел")]
        FlewAway,
        [Display(Name = "Прилетел")]
        FlewIn,
        [Display(Name = "Не отправлен")]
        NotSend,
        [Display(Name = "Не посажен")]
        NotLand,
        [Display(Name = "Авария")]
        Crash
    }
}
