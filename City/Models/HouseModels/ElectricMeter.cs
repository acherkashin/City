using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CyberCity.Models.HouseModels
{
    public class ElectricMeter
    {
        public ElectricMeter()
        {
            SpentPower = 0;
            CurrentPower = 0;
        }

        /// <summary>
        /// Текущее значение мощности
        /// </summary>
        public float CurrentPower { get; set; }

        /// <summary>
        /// потрачено за день в кВт
        /// </summary>
        public float SpentPower { get; set; }

        public void UpdateMeters(float electricPower)
        {
            if (electricPower != 0)
            {
                CurrentPower = electricPower;
            }

            SpentPower += CurrentPower;
        }


    }
   
}
