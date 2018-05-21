using Newtonsoft.Json;

namespace CyberCity.Models.WeatherStantion
{
    /// <summary>
    /// Класс погодных данных. 
    /// </summary>    
    public class OpenWeather
    {        
        /// <summary>
        /// Погода
        /// </summary>
        [JsonProperty("weather")]
        public Weather[] Weather;       

        /// <summary>
        /// Основные погодные данные
        /// </summary>
        [JsonProperty("main")]
        public Main Main;
        
        /// <summary>
        /// Данные о ветре
        /// </summary>
        [JsonProperty("wind")]
        public Wind Wind;       
    }
}
