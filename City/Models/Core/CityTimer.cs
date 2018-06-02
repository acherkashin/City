using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace CyberCity.Models.Core
{
    /// <summary>
    /// Время города
    /// </summary>
    public class CitySheduler
    {
        /// <remarks>
        /// Сохраняем все таймеры, чтобы их не удалил сборщик мусора
        /// </remarks>
        private List<Timer> timers = new List<Timer>();

        private DateTime _startTime;

        /// <summary>
        /// Время города
        /// </summary>
        public CityTime CurrentTime
        {
            get
            {
                //Продолжительность работы города
                var durrationCityRunned = Convert.ToInt64(DateTime.Now.Subtract(_startTime).TotalMilliseconds);
                var milSecInCityDay = CityHoursToRealMilliSecconds(24);
                //В городе нам не нужны дни и месяцы, а только часы и минуты - делим по модулю на 24 часа
                var currentTimeMilSec = Convert.ToInt64(durrationCityRunned % milSecInCityDay);
                // Переводим во время города 1 млСек Города = 1 60млСек реального времени
                var currentTime = TimeSpan.FromMilliseconds(currentTimeMilSec * 60);

                return new CityTime
                {
                    Hours = currentTime.Hours,
                    Minutes = currentTime.Minutes,
                };
            }
        }

        /// <summary>
        /// Запускает действие после прошествия укзанного количества часов - <paramref name="cityHours"/>
        /// </summary>
        /// <param name="cityHours">Интервал времени (1 час в городе = 1 мин. реального времени), через которое нужно запускать действие</param>
        /// <param name="action">Действие, которое необходимо запускать</param>
        public void RunEach(int cityHours, Action<CityTime> action)
        {
            var milliseconds = CityHoursToRealMilliSecconds(cityHours);
            Timer timer = new Timer((object a) => action(CurrentTime), null, 0, milliseconds);

            timers.Add(timer);
        }

        public void RunAt(int gameHours, Action<CityTime> action)
        {
            var startTime = CityHoursToRealMilliSecconds(gameHours);
            //повторять событие каждые 24 часа
            var repeatTime = CityHoursToRealMilliSecconds(24);

            Timer timer = new Timer((object a) => action(CurrentTime), null, startTime, repeatTime);

            timers.Add(timer);
        }

        public void Start()
        {
            _startTime = DateTime.Now;
        }

        /// <summary>
        /// Перевод игровых часов в реальное время
        /// </summary>
        private long CityHoursToRealMilliSecconds(int gameHours)
        {
            // измеряется в игровых часах 1 час в городе = 1 мин.реального времени
            var millisecons = Convert.ToInt64(TimeSpan.FromMinutes(gameHours).TotalMilliseconds);
            return millisecons;
        }
    }
}
