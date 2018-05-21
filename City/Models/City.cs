using CyberCity.Models.SubStation;

namespace CyberCity.Models
{
    public class City
    {
        private static City _instance;
        private NetHub _hub;

        public Station SubStation;
        public ReactorModel.Reactor Reactor;

        private City(NetHub hub)
        {
            _hub = hub;

            SubStation = new Station(_hub);
            Reactor = new ReactorModel.Reactor();

            SubStation.Start();
        }

        public void Start()
        {
            SubStation.Start();
        }

        public static City Create(NetHub hub)
        {
            return _instance = new City(hub);
        }

        public static City GetInstance()
        {
            if (_instance == null)
                throw new System.Exception($"Необходимо использовать метод {nameof(City.Create)} для создания экземпляра класса");
            return _instance;
        }
    }
}
