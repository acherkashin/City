using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace City.Models.HouseModels
{
    public class HouseRepository:IHouseRepository
    {
        private static ConcurrentDictionary<int, House> _houses = new ConcurrentDictionary<int, House>();

        public HouseRepository()
        {
            Add(new House() { Name = "Жилой дом 1", Id = 1, GasMeter = new GasMeter(), ElectricMeter = new ElectricMeter(), WaterMeter = new WaterMeter()});
            Add(new House() { Name = "Жилой дом 2", Id = 2, GasMeter = new GasMeter(), ElectricMeter = new ElectricMeter(), WaterMeter = new WaterMeter()});
            Add(new House() { Name = "Жилой дом 3", Id = 3, GasMeter = new GasMeter(), ElectricMeter = new ElectricMeter(), WaterMeter = new WaterMeter()});
            Add(new House() { Name = "Жилой дом 4", Id = 4, GasMeter = new GasMeter(), ElectricMeter = new ElectricMeter(), WaterMeter = new WaterMeter()});
        }

        public IEnumerable<House> GetAll()
        {
            return _houses.Values;
        }

        public void Add(House home)
        {
            _houses[home.Id] = home;
        }

        public House Find(int id)
        {
            House item;
            _houses.TryGetValue(id, out item);
            return item;
        }


        public void Update(House home)
        {
            _houses[home.Id] = home;
        }
    }
}
