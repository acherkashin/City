using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace CyberCity.Models.SubStation
{
    public class Station: ICityObject
    {
        private DataBus _bus;

        public Station(ApplicationContext context, DataBus bus)
        {
            _bus = bus;
        }

        public double Power { get; set; } = 0;
        public bool StateOfRele { get; set; } = false;
        public bool StateOSiren { get; set; } = false;

        public void ProcessPackage(Package package)
        {
            if (package.Method == "Siren")
            {

            }
        }

        /// <summary>
        /// Включение/Выключение реле
        /// </summary>
        public void ChangeRele(bool flagRele)
        {
            StateOfRele = flagRele;
            //ResultStRele = Convert.ToString(StateOfRele);
            _bus.SendStateChanged(Subject.Substation, GetState());
        }

        /// <summary>
        /// Включение сирены от Атомной станции
        /// </summary>
        public void UseSiren(bool flagSiren)
        {
            /// <summary>
            /// TODO: stateOfSiren необходимо передавать на Arduino для включения сирены
            /// </summary>   
            StateOSiren = flagSiren;
            //ResultStSiren = Convert.ToString(StateOSiren);
            _bus.SendStateChanged(Subject.Substation, GetState());
        }

        /// <summary>
        /// Передача энергии от Атомной станции
        /// </summary>
        public void GetPower(double value)
        {
            Power = value;
        }

        /// <summary>
        /// Отправка энергии в город
        /// </summary>
        public void SendPower()
        {
            /// <summary>
            /// TODO: Тут необходимо отправлять данные в город через хаб
            /// </summary>
            _bus.SendStateChanged(Subject.Substation, GetState());
        }

        public void Start()
        {
            new Task(() =>
            {
                while (true)
                {
                    //Power = Convert.ToString(s1.Power);
                    SendPower();
                    Thread.Sleep(3000);
                }
            }).Start();
        }

        public SubstationState GetState()
        {
            return new SubstationState
            {
                Power = Power,
                StRele = StateOfRele,
                StSiren = StateOSiren,
            };
        }
    }
}
