using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace City.Models.HouseModels
{
    public class House
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public bool IsOnLight { get; set; }

        public GasMeter GasMeter { get; set; }
        public WaterMeter WaterMeter { get; set; }
        public ElectricMeter ElectricMeter { get; set; }

        public House()
        {
            ElectricMeter = new ElectricMeter();
            WaterMeter = new WaterMeter();
            GasMeter = new GasMeter();
        }

        public void UpdateMeters(int electricPower = 0)
        {
            ElectricMeter.UpdateMeters(electricPower);
            WaterMeter.UpdateMeters();
            GasMeter.UpdateMeters();
        }

        public void UpdateTarifs(int gasTarif,int waterTarif,int electricTarif)
        {
            ElectricMeter.Tarif = electricTarif;
            WaterMeter.Tarif = waterTarif;
            GasMeter.Tarif = gasTarif;
        }
    }
}
