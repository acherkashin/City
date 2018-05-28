using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CyberCity.Models.HouseModels
{
    public class GasMeter
    {
        public GasMeter()
        {
            CurrentVolume = Generator.GenerateValue(0, 100);
            Tarif = Generator.GenerateValue(10, 50);
        }
        /// <summary>
        /// Потраченно воды за день в куб м
        /// </summary>
        public float CurrentVolume { get; set; }

        /// <summary>
        /// Тарифный коэффициент, обозначающий сколько стоит 1 куб м воды в рублях
        /// </summary>
        public float Tarif { get; set; }

        public void UpdateMeters()
        {
            CurrentVolume = Generator.GenerateValue(CurrentVolume, CurrentVolume + 10);
        }
    }
}
