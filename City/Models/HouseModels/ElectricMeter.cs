using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace City.Models.HouseModels
{
    public class ElectricMeter
    {
        public ElectricMeter()
        {
            SpentPower = Generator.GenerateValue(0,1000);
            CurrentPower = Generator.GenerateValue(0,100);
            Tarif = Generator.GenerateValue(1,10);
        }

        /// <summary>
        /// Текущее значение мощности
        /// </summary>
        public int CurrentPower { get; set; }

        /// <summary>
        /// потрачено за день в кВт
        /// </summary>
        public int SpentPower{ get; set; }

        /// <summary>
        /// Тарифный коэффициент, обозначающий сколько стоит 1 кВт в рублях
        /// </summary>
        public int Tarif { get; set; }

    }
   
}
