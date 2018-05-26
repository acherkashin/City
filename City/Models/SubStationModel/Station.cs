using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

namespace CyberCity.Models.SubStationModel
{
    public class SubStation: ICityObject
    {
        private DataBus _bus;

        public SubStation(ApplicationContext context, DataBus bus)
        {
            _bus = bus;
        }

        public double Power { get; set; } = 0;
        public bool IsOnRele { get; set; } = false;
        public bool IsOnSiren { get; set; } = false;

        public void ProcessPackage(Package package)
        {
            if (package.Method == "OnSiren")
            {
                IsOnSiren= Newtonsoft.Json.JsonConvert.DeserializeObject<bool>(package.Params);
                UseSiren();
            }
            if (package.Method == "GetPower")
            {
                double ParseData = Newtonsoft.Json.JsonConvert.DeserializeObject<double>(package.Params);
                GetPower(ParseData);
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
            /// <summary>
            /// TODO: IsOnSiren необходимо передавать на Arduino для включения сирены
            /// Конвертация из True в "1" и из False в "0" необходима по просьбе программистов Arduino
            /// </summary>
            string isSiren;
            if (IsOnSiren)
            {
                isSiren = "1";
            }
            else
            {
                isSiren = "0";
            }
            SendDataToArduino("NameMethodOfSiren", isSiren);
            _bus.SendStateChanged(Subject.Substation, GetState());
        }

        /// <summary>
        /// Получение энергии от Атомной станции
        /// </summary>
        public void GetPower(double value)
        {
            Power = value;
        }

        /// <summary>
        /// Отправка энергии в город
        /// </summary>
        public void SendPower(double sendPower)
        {
            ///<commit>
            ///TODO: Нужно узнать точное имя метода в объекте "Город"
            /// </commit>
            _bus.Send(new Package()
            {
                From = Subject.Substation,
                To = Subject.Houses,
                Method = "NameMethodOfGetPowerInHouses",
                Params = Newtonsoft.Json.JsonConvert.SerializeObject(sendPower),
            });
            _bus.SendStateChanged(Subject.Substation, GetState());
        }

        public void Start()
        {
            new Task(() =>
            {
                while (true)
                {
                    if (IsOnRele)
                    {
                        SendPower(Power);
                    }
                    else
                    {
                        SendPower(0);
                    }
                    Thread.Sleep(60000);
                }
            }).Start();
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
        /// <param name="Method">Название метода</param>
        /// <param name="p">Параметр</param>
        public void SendDataToArduino(String Method, string p)
        {
            try
            {
                ///TODO: Вместо # необходимо вписывать IP соответствующего объекта
                String URL = "http://192.168.#.#/" + Method + "?p=" + p;
                WebRequest request = WebRequest.Create(URL);
                request.Method = "POST";
                request.ContentType = "application/x-www-form-urlencoded";
                WebResponse response = request.GetResponse();
                response.Close();
            }
            catch (Exception ex)
            {

            }
        }
    }
}
