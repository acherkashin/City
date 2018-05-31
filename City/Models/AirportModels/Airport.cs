﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CyberCity.Models.AirportModels
{
    public class Airport : CityObject
    {


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
            throw new NotImplementedException();
        }




        public FlightStates flightStates { get; set; }
        public LightStates lightStates { get; set; }


        public bool Send { get; set; } = false;
        public bool Land { get; set; } = false;

        /// <summary>
        /// Пассажиры, отправленные в полет
        /// </summary>
        public virtual ICollection<Passenger> Passengers { get; set; }

        // отправить самолет
        public void SendPlane()
        {
            Send = true;
            // отправить запрос на разрешение полета в метеостанцию
        }

        // посадить самолет
        public void LandPlane()
        {
            Land = true;
            // отправить запрос на разрешение посадки в метеостанцию
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
            if (resolution && Send)
            {
                //1.Рандомно набрать в Airport.Passangers 3 человека(перед этим очистить список)
                //2.Отправить запрос в банк на оплату билетов
                flightStates = FlightStates.FlewAway;
            }
            else if (resolution && Land) flightStates = FlightStates.FlewIn;
            else if (!resolution && Send) flightStates = FlightStates.NotSend;
            else if (!resolution && Land) flightStates = FlightStates.NotLand;
            else throw new Exception();
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
