namespace CyberCity.Models.WeatherStantionModel
{
    public class WeatherStantion: ICityObject
    {
        /// <summary>
        /// Включена ли станция.
        /// </summary>
        public bool IsOn = true;

        public void ProcessPackage(Package package)
        {
            throw new System.NotImplementedException();
        }
    }
}
