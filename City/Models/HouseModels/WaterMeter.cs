using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CyberCity.Models.HouseModels
{
    public class WaterMeter
    {
        public WaterMeter()
        {
            CurrentVolume = 0;
        }

        /// <summary>
        /// Потраченно газа за день в куб м
        /// </summary>
        public float CurrentVolume { get; set; }

        public void UpdateMeters()
        {
            CurrentVolume = Generator.GenerateValue(CurrentVolume, CurrentVolume + 10);
        }
    }
}
