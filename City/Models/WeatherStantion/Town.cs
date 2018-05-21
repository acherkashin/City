using System.Runtime.Serialization;

namespace CyberCity.Models.WeatherStantion
{
    /// <summary>
    /// Город.
    /// </summary>
    [DataContract(Name = "city")]
    public class Town
    {
        /// <summary>
        /// Название города
        /// </summary>
        [DataMember(Name = "name")]
        public string Name { get; set; }

        /// <summary>
        /// Ключ
        /// </summary>
        [DataMember(Name = "key")]
        public string Key { get; set; }
    }
}