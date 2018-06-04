using CyberCity.Models.BankModel;
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

        public Random rand = new Random();
        /// <summary>
        /// люди из базы данных
        /// </summary>
        List<Resident> ClientsOfBank = new List<Resident>();

        public Airport(DataBus bus) : base(bus)
        {
            var clients = _context.Residents;
            
            foreach (var person in clients)
            {
                ClientsOfBank.Add(person);
            }

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
                    _bus.SendStateChanged(Subject.Airport, null);
                }
                else flightState = FlightStates.NotSend;
                _bus.SendStateChanged(Subject.Airport, null);
            }
            else if (package.Method == CanLandMethod)
            {
                var canLand = Newtonsoft.Json.JsonConvert.DeserializeObject<bool>(package.Params);
                if (canLand) flightState = FlightStates.FlewIn;
                else flightState = FlightStates.NotLand;
                _bus.SendStateChanged(Subject.Airport, null);
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
        public virtual List<Passenger> Passengers { get; set; }

        /// <summary>
        /// отправить самолет
        /// </summary>
        public void SendPlane()
        {
            int count = ClientsOfBank.Count();
            Passengers.Clear();
            for (var i = 0; i<3; i++)
            {
                try
                {
                    int id = rand.Next(count);
                    Resident res = ClientsOfBank.Where(t => t.Id == id).Single();
                    Passengers.Add(new Passenger { Id = res.Id, Name = res.Surname + res.Name + res.Patronymic });
                }
                catch (Exception e)
                {
                }
               
            }

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
            if (GetUser() != null)
            {
                string url = $"{GetUser().ArduinoUrl}/method?";
                _bus.SendToArduino(url);
            }
        }

        /// <summary>
        /// выключить свет
        /// </summary>
        public void TurnOffLight()
        {
            if (GetUser() != null)
            {
                lightState = LightStates.TurnedOff;
                //передать ардуино, чтобы выключился свет
                string url = $"{GetUser().ArduinoUrl}/method?";
                _bus.SendToArduino(url);
            }
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
