namespace CyberCity.Models.WeatherStantionModel
{
    /// <summary>
    /// Результат запроса. 
    /// </summary>
    public class WeaherResult
    {
        /// <summary>
        /// Температура.
        /// </summary>
        public double Tempreture { get; set; }

        /// <summary>
        /// Давление воздуха. 
        /// </summary>
        public double Pressure { get; set; }

        /// <summary>
        /// Ветер.
        /// </summary>
        public double Wind { get; set; }

        /// <summary>
        /// Влажность.
        /// </summary>
        public double Humidity { get; set; }

        /// <summary>
        /// Картинка. 
        /// </summary>
        public string Icon { get; set; }
    }
}