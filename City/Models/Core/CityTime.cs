namespace CyberCity.Models.Core
{
    public class CityTime
    {
        public int Hours { get; set; }
        public int Minutes { get; set; }

        public override string ToString()
        {
            return $"{Hours}:{Minutes}";
        }
    }
}
