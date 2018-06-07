using CyberCity.Models.MunicipalityModel;
using CyberCity.Models.SubStationModel;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

namespace CyberCity.Models.HouseModels
{
    public class Houses : CityObject
    {
        public const string SendMetricsMethod = "SendMetrics";
        public const string GetPowerMethod = "GetPower";

        /// <summary>
        /// Жилые комплексы
        /// </summary>
        public List<House> Homes { get; set; }

        /// <summary>
        /// Тарифные коэффициенты, получаемые от Муниципалитета
        /// </summary>
        public Tarifs Tarifs { get; set; }

        /// <summary>
        /// Мощность, полученная от подстанции, действует на все дома
        /// </summary>
        public float CityPower { get; set; }

        public Houses(DataBus bus) : base(bus)
        {
            Homes = new List<House>();
            Homes.Add(new House() { Name = "Жилой комплекс 1", Id = 1, IP = "http://192.168.1.0", GasMeter = new GasMeter(), ElectricMeter = new ElectricMeter(), WaterMeter = new WaterMeter() });
            Homes.Add(new House() { Name = "Жилой комплекс 2", Id = 2, IP = "http://192.168.1.1", GasMeter = new GasMeter(), ElectricMeter = new ElectricMeter(), WaterMeter = new WaterMeter() });

            Tarifs = new Tarifs() { Gas = 1, Water = 1, Electric = 1 };
            CityPower = 100;
        }

        /// <summary>
        /// Метод для отправки пакетов другим объектам города
        /// </summary>
        public void Start()
        {
            List<HouseMeter> metrics = GetMetrics();
            new Task(() =>
            {
                _bus.Send(new Package()
                {
                    From = Subject.Houses,
                    To = Subject.Bank,
                    Method = SendMetricsMethod,
                    Params = JsonConvert.SerializeObject(metrics), 
                });
                Thread.Sleep(1000);
            }).Start();
        }

        /// <summary>
        /// Формирует данные о том, сколько должен заплатить каждый дом за потраченные ресурсы
        /// </summary>
        /// <returns></returns>
        public List<HouseMeter> GetMetrics()
        {
            List<HouseMeter> homeMeters = new List<HouseMeter>();
            foreach (var home in Homes)
            {
                float meters = home.GasMeter.CurrentVolume * Tarifs.Gas +
                               home.ElectricMeter.SpentPower * Tarifs.Electric +
                               home.WaterMeter.CurrentVolume * Tarifs.Water;
                homeMeters.Add(new HouseMeter { IdHome = home.Id, Summa = meters });
            }
            return homeMeters;
        }

        /// <summary>
        /// Обрабатывает пакеты с данными, полученные от других объектов города
        /// </summary>
        public override void ProcessPackage(Package package)
        {
            if (package.Method == Municipality.UpdateTarifMethod)
            {
                var tarifs = JsonConvert.DeserializeObject<Tarifs>(package.Params);

                foreach(var house in Homes)
                {
                    Tarifs.Electric = tarifs.Electric;
                    Tarifs.Gas = tarifs.Gas;
                    Tarifs.Water = tarifs.Water;
                }

            }
            else if (package.Method == SubStation.PowerInHousesMethod)
            {
                CityPower = JsonConvert.DeserializeObject<float>(package.Params);
                SwitchLight();
                _bus.SendStateChanged(Subject.Houses, null);

            }
        }

        /// <summary>
        /// Вкл/выкл свет в жилых комплексах, в зависимости от того, сколько мощности пришло от подстанции
        /// </summary>
        private void SwitchLight()
        {
            //пришло слишком много мощности, поэтому отрубаем свет во всех домах
            if (CityPower > 100)
            {
                foreach (var house in Homes)
                {
                    house.IsOnLight = false;
                    house.UpdateMeters(0);
                }
            }
            else if (CityPower <= 100 && CityPower >= 50)
            {
                foreach (var house in Homes)
                {
                    house.IsOnLight = true;
                    house.UpdateMeters(CityPower);
                }
            }
            else if (CityPower < 50 && CityPower >= 25)
            {
                Homes.Find(x=>x.Id == 1).IsOnLight = false;
                Homes.Find(x => x.Id == 1).UpdateMeters(0);

                Homes.Find(x => x.Id == 2).IsOnLight = true;
                Homes.Find(x => x.Id == 1).UpdateMeters(CityPower);

            }
            //пришло слишком мало мощности, поэтому отрубаем свет во всех домах
            else if (CityPower < 25)
            {
                foreach (var house in Homes)
                {
                    house.IsOnLight = false;
                    house.UpdateMeters(0);
                }
            }

            SwitchLightOnArduino();
        }

        /// <summary>
        /// Переключение света в конкретном доме
        /// </summary>
        public void SwitchLightInHouse(int id, bool isOnLight)
        {
            if( CityPower < 100 || CityPower >= 25)
            {
                Homes.Find(x => x.Id == id).IsOnLight = isOnLight;
            }
        }

        /// <summary>
        /// Передача команды для переключения света контроллеру Ардуино
        /// </summary>
        public void SwitchLightOnArduino()
        {
            if (GetUser() == null)
                return;

            try
            {
                //TODO Лукина: уточнить, как получить ip для каждого жилого комплекса
                //string urlToArduino = GetUser().ArduinoUrl;

                string switchLightCommand = "";

                foreach (var home in Homes)
                {
                    if (home.IsOnLight)
                    {
                        switch (home.ColorMode)
                        {
                            case ColorModes.White:
                                switchLightCommand =
                                    ArduinoCommand.CommandDictionary.GetValueOrDefault(ArduinoCommands.LedOn);
                                break;
                            case ColorModes.Blue:
                                switchLightCommand =
                                    ArduinoCommand.CommandDictionary.GetValueOrDefault(ArduinoCommands.LedOnBlue);
                                break;
                            case ColorModes.Green:
                                switchLightCommand =
                                    ArduinoCommand.CommandDictionary.GetValueOrDefault(ArduinoCommands.LedOnGreen);
                                break;
                            case ColorModes.Red:
                                switchLightCommand =
                                    ArduinoCommand.CommandDictionary.GetValueOrDefault(ArduinoCommands.LedOnRed);
                                break;
                            case ColorModes.Random:
                                switchLightCommand =
                                    ArduinoCommand.CommandDictionary.GetValueOrDefault(ArduinoCommands.LedOnRnd);
                                break;
                        }
                    }
                    else
                    {
                        switchLightCommand =
                            ArduinoCommand.CommandDictionary.GetValueOrDefault(ArduinoCommands.LedOff);
                    }
                    string url = home.IP + $"/{ switchLightCommand}";
                    WebRequest request = WebRequest.Create(url);
                    request.Method = "GET";
                    WebResponse response = request.GetResponse();
                }
            }
            catch (Exception ex)
            {

            }
        }
    }
}
