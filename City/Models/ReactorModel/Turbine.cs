using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace CyberCity.Models.Reactor
{
    public class Turbine
    {
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
