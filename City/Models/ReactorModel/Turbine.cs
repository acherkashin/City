using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

namespace CyberCity.Models.Reactor
{
    public class Turbine
    {
        /// <summary>
        /// Название метода на Arduino для включения/выключения турбины
        /// </summary>
        public const string ArduinoOnOffTurbineMethod = "OnOffTurbine";
        public bool IsOnSiren { get; set; } = false;
        /// <summary>
        /// Состояние турбины
        /// </summary>
        public bool IsOnTurbine { get; set; } = false;
        /// <summary>
        /// Текущие обороты турбины
        /// </summary>
        public double currentRPM { get; set; }
        /// <summary>
        /// Значение, при котором происходит увеличение вибрации турбины
        /// </summary>
        private int maxRPM = 3200;
        public int MaxRPM
        {
            get { return maxRPM; }
        }
        /// <summary>
        /// Значение, после которого энергия начинает вырабатываться на 100 процентов
        /// </summary>
        private int minRPM = 2500;
        public int MinRPM
        {
            get { return minRPM; }
        }
        /// <summary>
        /// Текущяя вибрация турбины
        /// </summary>
        public double currentVibration { get; set; }
        /// <summary>
        /// Значение после которого происходит поломка турбины на 150 секунд
        /// </summary>
        private int maxVibration = 300;
        public int MaxVibration
        {
            get { return maxVibration; }
        }
        /// <summary>
        /// Состояние поломки: поломана турбина или нет
        /// </summary>
        private bool isBroken = false;
        public bool IsBroken
        {
            get { return isBroken; }
            set { isBroken = value; }
        }

        /// <summary>
        /// Запуск турбины
        /// </summary>
        public void Start()
        {
            IsBroken = false;
            IsOnTurbine = true;
            IsOnSiren = false;
            OnTurbineOnArduino();

        }

        /// <summary>
        /// Запуск турбины на Arduino
        /// </summary>
        private void OnTurbineOnArduino()
        {
            try
            {
                ///<summary>
                ///TODO: Вместо # необходимо вписывать IP соответствующего объекта
                ///IP 192.168.0.2
                ///</summary>
                String URL = "http://192.168.0.2/" + ArduinoOnOffTurbineMethod + "?p=1";
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

        /// <summary>
        /// Остановка турбины
        /// </summary>
        public void Stop()
        {
            TimerOfBroken();
            IsOnTurbine = false;
            IsBroken = true;
            currentRPM = 0;
            currentVibration = 0;
            IsOnSiren = true;
        }

        /// <summary>
        /// Таймер, отвечающий за остановку турбины на 150(150000 ms) секунд при поломке
        /// </summary>
        public void TimerOfBroken()
        {
            TimerCallback tm = new TimerCallback(RepareTurbine);
            Timer timer = new Timer(tm, null, 150000, -1);
        }

        /// <summary>
        /// Запуск турбины после починки
        /// </summary>
        private void RepareTurbine(object obj)
        {
            Start();
            
        }
    }
}
