using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace CyberCity.Models.AirportModels
{
    public enum LightStates
    {
        [Display(Name = "Включен")]
        TurnedOn,
        [Display(Name = "Выключен")]
        TurnedOff
    }
}
