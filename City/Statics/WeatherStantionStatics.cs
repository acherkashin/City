using System;

namespace CyberCity.Statics
{
    /// <summary>
    /// Включена ли станция.
    /// </summary>
    static public class WeatherStantionStatics
    {
        /// <summary>
        /// Включена ли станция.
        /// </summary>
        public static bool IsOn = true;

        /// <summary>
        /// Число запросов, после которого отправить ответ о неблагоприятных условиях.
        /// </summary>
        public static int SendSmash = Random.Next(3, 10);

        /// <summary>
        /// Номер текущего ответа. 
        /// </summary>
        public static int CurrentAnswerNumber = 0;

        /// <summary>
        /// Генератор случайных чисел. 
        /// </summary>
        public static Random Random = new Random(); 
    }
}
