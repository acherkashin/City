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
            CurrentVolume = Generator.GenerateValue(0, 100);
            Tarif = Generator.GenerateValue(10, 250);
        }

        /// <summary>
        /// Потраченно газа за день в куб м
        /// </summary>
        public float CurrentVolume { get; set; }

        /// <summary>
        /// Тарифный коэффициент, обозначающий сколько стоит 1 куб м газа в рублях
        /// </summary>
        public float Tarif { get; set; }

        public void UpdateMeters()
        {
            CurrentVolume = Generator.GenerateValue(CurrentVolume, CurrentVolume + 20);
        }
    }
}
