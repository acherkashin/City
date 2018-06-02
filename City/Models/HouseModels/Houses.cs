using CyberCity.Models.MunicipalityModel;
using CyberCity.Models.SubStationModel;
using Newtonsoft.Json;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

namespace CyberCity.Models.HouseModels
{
    public class Houses : CityObject
    {
        public const string SendMetricsMethod = "SendMetrics";
        public const string GetPowerMethod = "GetPower";

        private ConcurrentDictionary<int, House> _houses = new ConcurrentDictionary<int, House>();

        public Houses(DataBus bus) : base(bus)
        {
            Add(new House() { Name = "Жилой дом 1", Id = 1, GasMeter = new GasMeter(), ElectricMeter = new ElectricMeter(), WaterMeter = new WaterMeter() });
            Add(new House() { Name = "Жилой дом 2", Id = 2, GasMeter = new GasMeter(), ElectricMeter = new ElectricMeter(), WaterMeter = new WaterMeter() });
            Add(new House() { Name = "Жилой дом 3", Id = 3, GasMeter = new GasMeter(), ElectricMeter = new ElectricMeter(), WaterMeter = new WaterMeter() });
            Add(new House() { Name = "Жилой дом 4", Id = 4, GasMeter = new GasMeter(), ElectricMeter = new ElectricMeter(), WaterMeter = new WaterMeter() });
        }

        public void Start()
        {
            new Task(() =>
            {
                _bus.Send(new Package()
                {
                    From = Subject.Houses,
                    To = Subject.Bank,
                    Method = SendMetricsMethod,
                    Params = "", //TODO Черкашин: добавить метрики
                });

                Thread.Sleep(1000);
            }).Start();
        }

        public override void ProcessPackage(Package package)
        {
            if (package.Method == Municipality.UpdateTarifMethod)
            {
                //TODO Черкашин: Обновить тарифы в домах
                var tarifs = JsonConvert.DeserializeObject<Tarifs>(package.Params);

            }
            else if (package.Method == SubStation.PowerInHousesMethod)
            {
                float power = JsonConvert.DeserializeObject<float>(package.Params);
                ChangePower(power);

                //TDOO Черкашин: Добавить обновление вьюхи
                _bus.SendStateChanged(Subject.Houses, null);
            }
        }

        private void ChangePower(float power)
        {
            var houses = GetAll();


            //пришло слишком много мощности, поэтому отрубаем свет во всех домах
            if (power > 100)
            {
                foreach (var house in houses)
                {
                    house.IsOnLight = false;

                }
            }
            else if (power <= 100 && power >= 75)
            {
                foreach (var house in houses)
                {
                    house.IsOnLight = true;
                }
            }
            else if (power < 75 && power >= 50)
            {
                Find(1).IsOnLight = false;

                Find(2).IsOnLight = true;
                Find(3).IsOnLight = true;
                Find(4).IsOnLight = true;

            }
            else if (power < 50 && power >= 25)
            {
                Find(1).IsOnLight = false;
                Find(2).IsOnLight = false;

                Find(3).IsOnLight = true;
                Find(4).IsOnLight = true;
            }
            else if (power < 25 && power >= 10)
            {
                Find(1).IsOnLight = false;
                Find(2).IsOnLight = false;
                Find(3).IsOnLight = false;

                Find(4).IsOnLight = true;
            }
            //пришло слишком мало мощности, поэтому отрубаем свет во всех домах
            else if (power < 10)
            {
                foreach (var house in houses)
                {
                    house.IsOnLight = false;
                }
            }

            SwitchLightOnArduino();
        }

        public void SwitchLightOnArduino()
        {
            if (GetUser() == null)
                return;

            try
            {
                var homes = GetAll();
                
                string urlToArduino = GetUser().ArduinoUrl;

                foreach (var home in homes)
                {
                    string switchLightCommand = home.IsOnLight
                        ? ArduinoCommand.CommandDictionary.GetValueOrDefault(ArduinoCommands.LedOn)
                        : ArduinoCommand.CommandDictionary.GetValueOrDefault(ArduinoCommands.LedOff);
                    WebRequest request = WebRequest.Create(urlToArduino + $"${switchLightCommand}?id=${home.Id}&${home.IsOnLight}");
                    request.Method = "GET";
                    WebResponse response = request.GetResponse();

                    //TODO: можно использовать _bus.SendToArduino()
                }
            }
            catch (Exception ex)
            {

            }
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
