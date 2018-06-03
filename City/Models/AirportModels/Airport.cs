using CyberCity.Models.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace CyberCity.Models.AirportModels
{
    public class Airport : CityObject
    {
        //TODO Черкашин: Описать все поля
        /// <summary>
        /// Запрос разрешения на полет
        /// </summary>
        public const string CanFlyMethod = "CanFly";
        /// <summary>
        /// Запрос на приземление
        /// </summary>
        public const string CanLandMethod = "CanLand";
        /// <summary>
        /// Отправить данные о пассажирах в банк
        /// </summary>
        public const string AirportInvoiceMethod = "AirportInvoice";

        public Airport(DataBus bus) : base(bus)
        {
            Passengers = new List<Passenger> {
                new Passenger{ Id=1,Name = "Петров И.И" },
                new Passenger{ Id=2,Name = "Иванов И.И" },
                new Passenger{ Id=3,Name = "Соколов И.И" }
            };

            flightState = FlightStates.NotSend;
            lightState = LightStates.TurnedOff;
        }


        /// <summary>
        /// обработка пакетов
        /// </summary>
        /// <param name="package"></param>

        public override void ProcessPackage(Package package)
        {
            if (package.Method == CanFlyMethod)
            {
                var canFly = Newtonsoft.Json.JsonConvert.DeserializeObject<bool>(package.Params);

                if (canFly)
                {
                    _bus.Send(new Package()
                    {
                        From = Subject.Airport,
                        To = Subject.Bank,
                        Method = AirportInvoiceMethod,
                        Params = Newtonsoft.Json.JsonConvert.SerializeObject(null),//TODO : заменить null на информацию о пассажирах
                    });
                    flightState = FlightStates.FlewAway;
                }
                else flightState = FlightStates.NotSend;
            }
            else if (package.Method == CanLandMethod)
            {
                var canLand = Newtonsoft.Json.JsonConvert.DeserializeObject<bool>(package.Params);
                if (canLand) flightState = FlightStates.FlewIn;
                else flightState = FlightStates.NotLand;
            }
        }
        /// <summary>
        /// Состояние полета
        /// </summary>
        public FlightStates flightState { get; set; }
        /// <summary>
        /// Состояние света на взлетной полосе
        /// </summary>
        public LightStates lightState { get; set; }

        /// <summary>
        /// Пассажиры, отправленные в полет
        /// </summary>
        public virtual ICollection<Passenger> Passengers { get; set; }
        /// <summary>
        /// отправить самолет
        /// </summary>
        public void SendPlane()
        {
            //    //1.Рандомно набрать в Airport.Passangers 3 человека(перед этим очистить список)
            _bus.Send(new Package()
            {
                From = Subject.Airport,
                To = Subject.WeatherStation,
                Method = CanFlyMethod,
            });
        }
        /// <summary>
        /// посадить самолет
        /// </summary>
        public void LandPlane()
        {
            _bus.Send(new Package()
            {
                From = Subject.Airport,
                To = Subject.WeatherStation,
                Method = CanLandMethod,
            });
        }
        /// <summary>
        /// включить свет
        /// </summary>
        public void TurnOnLight()
        {
            lightState = LightStates.TurnedOn;
            //передать ардуино, чтобы включился свет
        }
        /// <summary>
        /// выключить свет
        /// </summary>
        public void TurnOffLight()
        {
            lightState = LightStates.TurnedOff;
            //передать ардуино, чтобы выключился свет
        }

       
        /// <summary>
        /// Проверяет, пришло ли время отправлять или приземлять самолет, включать свет
        /// </summary>
        /// <param name="Time"> Время в часах.</param>
        public void IsTime(int Time)
        {
            switch (Time)
            {
                case (9):
                    SendPlane();
                    break;
                case (10):
                    TurnOnLight();
                    break;
                case (18):
                    TurnOnLight();
                    break;
                case (19):
                    LandPlane();
                    break;
            }
        }

        public void Work(CityTime dateTime)
        {
            IsTime(dateTime.Hours);
        }
    }
}
