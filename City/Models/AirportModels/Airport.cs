using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CyberCity.Models.AirportModels
{
    public class Airport : CityObject
    {
        //TODO Черкашин: Описать все поля
        public const string CanFlyMethod = "CanFly";
        public const string CanLandMethod = "CanLand";
        /// <summary>
        /// Отправитьданны о пассажирах в банк
        /// </summary>
        public const string AirportInvoiceMethod = "AirportInvoice";

        public Airport(ApplicationContext context, DataBus bus) : base(context, bus)
        {
            Passengers = new List<Passenger> {
                new Passenger{ Id=1,Name = "Петров И.И" },
                new Passenger{ Id=2,Name = "Иванов И.И" },
                new Passenger{ Id=3,Name = "Соколов И.И" }
            };

            flightStates = FlightStates.NotSend;
            lightStates = LightStates.TurnedOff;
        }

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
                }
            }
            else if (package.Method == CanLandMethod)
            {
                //TODO: Добавить обработку посадки
            }
        }

        public FlightStates flightStates { get; set; }
        public LightStates lightStates { get; set; }

        /// <summary>
        /// Пассажиры, отправленные в полет
        /// </summary>
        public virtual ICollection<Passenger> Passengers { get; set; }

        // отправить самолет
        public void SendPlane()
        {
            _bus.Send(new Package()
            {
                From = Subject.Airport,
                To = Subject.WeatherStation,
                Method = CanFlyMethod,
            });
        }

        // посадить самолет
        public void LandPlane()
        {
            _bus.Send(new Package()
            {
                From = Subject.Airport,
                To = Subject.WeatherStation,
                Method = CanLandMethod,
            });
        }

        //включить свет
        public void TurnOnLight()
        {
            lightStates = LightStates.TurnedOn;
            //передать ардуино, чтобы включился свет
        }

        //выключить свет
        public void TurnOffLight()
        {
            lightStates = LightStates.TurnedOff;
            //передать ардуино, чтобы выключился свет
        }

        // прием разрешения на вылет/прилет
        public void AllowFlight(bool resolution)
        {
            //if (resolution && Send)
            //{
            //    //1.Рандомно набрать в Airport.Passangers 3 человека(перед этим очистить список)
            //    //2.Отправить запрос в банк на оплату билетов
            //    flightStates = FlightStates.FlewAway;
            //}
            //else if (resolution && Land) flightStates = FlightStates.FlewIn;
            //else if (!resolution && Send) flightStates = FlightStates.NotSend;
            //else if (!resolution && Land) flightStates = FlightStates.NotLand;
            //else throw new Exception();
        }


        // Вызывает обработку события по времени
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
    }
}
