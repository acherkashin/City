using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CyberCity.Models.HouseModels
{
    public class House
    {

        public int Id { get; set; }
        public string Name { get; set; }
        public bool IsOnLight { get; set; }
        public ColorModes ColorMode { get; set; }

        public GasMeter GasMeter { get; set; }
        public WaterMeter WaterMeter { get; set; }
        public ElectricMeter ElectricMeter { get; set; }

        public House()
        {
            ElectricMeter = new ElectricMeter();
            WaterMeter = new WaterMeter();
            GasMeter = new GasMeter();
        }

        public void UpdateMeters(float electricPower = 0)
        {
            ElectricMeter.UpdateMeters(electricPower);
            GasMeter.UpdateMeters();
            WaterMeter.UpdateMeters();
        }
    }
}
