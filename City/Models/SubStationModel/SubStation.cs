using CyberCity.Models.Core;
using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

namespace CyberCity.Models.SubStationModel
{
    public class SubStation : CityObject
    {
        /// <summary>
        /// Метод для принятия запроса на включения сирены на Arduino
        /// </summary>
        public const string OnSirenMethod = "OnSiren";
        /// <summary>
        /// Метод для получения энергии с объекта "Атомная станция"
        /// </summary>
        public const string GetPowerMethod = "GetPower";
        /// <summary>
        /// Метод для отправки мощности на объект "Жилые дома"
        /// </summary>
        public const string PowerInHousesMethod = "PowerInHouses";

        public SubStation(ApplicationContext context, DataBus bus) : base(context, bus) { }

        public double Power { get; set; } = 0;
        public bool IsOnRele { get; set; } = false;
        public bool IsOnSiren { get; set; } = false;

        public override void ProcessPackage(Package package)
        {
            if (package.Method == OnSirenMethod)
            {
                IsOnSiren = Newtonsoft.Json.JsonConvert.DeserializeObject<bool>(package.Params);
                UseSiren();
            }
            if (package.Method == GetPowerMethod)
            {
                double ParseData = Newtonsoft.Json.JsonConvert.DeserializeObject<double>(package.Params);
                SetPower(ParseData);
            }
        }

        /// <summary>
        /// Включение/Выключение реле
        /// </summary>
        public void ChangeRele(bool IsRele)
        {
            IsOnRele = IsRele;
            _bus.SendStateChanged(Subject.Substation, GetState());
        }

        /// <summary>
        /// Включение сирены от Атомной станции
        /// </summary>
        public void UseSiren()
        {
            // IsOnSiren необходимо передавать на Arduino для включения сирены
            // Конвертация из True в "1" и из False в "0" необходима по просьбе программистов Arduino
            string isSiren = IsOnSiren ? "1": "0";

            SendToArduino("NameMethodOfSiren", isSiren);
            _bus.SendStateChanged(Subject.Substation, GetState());
        }

        /// <summary>
        /// Установка энергии, полученной от Атомной станции
        /// </summary>
        public void SetPower(double value)
        {
            Power = value;
        }

        /// <summary>
        /// Отправка энергии в город
        /// </summary>
        public void SendPower(double sendPower)
        {
            _bus.Send(new Package()
            {
                From = Subject.Substation,
                To = Subject.Houses,
                Method = PowerInHousesMethod,
                Params = Newtonsoft.Json.JsonConvert.SerializeObject(sendPower),
            });
            _bus.SendStateChanged(Subject.Substation, GetState());
        }

        public void Work(CityTime time)
        {
            if (IsOnRele)
            {
                SendPower(Power);
            }
            else
            {
                SendPower(0);
            }

        }

        public SubstationState GetState()
        {
            return new SubstationState
            {
                Power = Power,
                StRele = IsOnRele,
                StSiren = IsOnSiren,
            };
        }

        /// <summary>
        /// Отправка данных на Arduino
        /// </summary>
        /// <param name="method">Название метода</param>
        /// <param name="p">Параметр</param>
        private void SendToArduino(string method, string p)
        {
            if (GetUser() == null)
                return;

            string url = $"{GetUser().ArduinoUrl}/method?p={p}";
            _bus.SendToArduino(url);
        }
    }
}
