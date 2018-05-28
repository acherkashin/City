using Newtonsoft.Json;

namespace CyberCity.Models.WeatherStantionModel
{
    /// <summary>
    /// Отображение погодных условий. 
    /// </summary>
    [JsonObject("weather")]
    public class Weather
    {
        /// <summary>
        /// Идентификатор.
        /// </summary>
        [JsonProperty("id")]
        public int Id;
        
        [JsonProperty("main")]
        public string Main;

        [JsonProperty("description")]
        public string Description;

        /// <summary>
        /// Изображение
        /// </summary>
        [JsonProperty("icon")]
        public string Icon;       
    }
}
