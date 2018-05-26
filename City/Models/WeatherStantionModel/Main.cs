using Newtonsoft.Json;

namespace CyberCity.Models.WeatherStantionModel
{
    /// <summary>
    /// Основные метеоданные.
    /// </summary>
    [JsonObject("main")]
    public class Main
    {
        private double _tempreture;

        /// <summary>
        /// Температура
        /// </summary>
        [JsonProperty("temp")]
        public double Tempreture
        {
            get
            {
                return _tempreture;
            }
            set
            {
                _tempreture = value - 273;
            }
        }

        /// <summary>
        /// Давление
        /// </summary>
        [JsonProperty("pressure")]
        private double _pressure;

        public double Pressure
        {
            get
            {
                return _pressure;
            }
            set
            {
                _pressure = value / 1.3332239;
            }
        }

        /// <summary>
        /// Влажность
        /// </summary>
        [JsonProperty("humidity")]
        public double Humidity;

        private double _maxTempreture;

        /// <summary>
        /// Максимально наблюдаемая температура.
        /// </summary>
        [JsonProperty("temp_max")]
        public double MaxTempreture
        {
            get
            {
                return _tempreture;
            }
            set
            {
                _tempreture = value - 273;
            }
        }        

        private double _minTempreture;

        /// <summary>
        /// Минимально наблюдаемая температура
        /// </summary>
        [JsonProperty("temp_min")]
        public double MinTempreture
        {
            get
            {
                return _tempreture;
            }
            set
            {
                _tempreture = value - 273;
            }
        }
    }
}
