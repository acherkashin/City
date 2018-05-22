using CyberCity.Models.ReactorModel;
using CyberCity.Models.SubStation;

namespace CyberCity.Models
{
    public class City
    {
        private static City _instance;
        private NetHub _hub;
        private ApplicationContext _context;

        public Station SubStation { get; set; }
        public NuclearStation NuclearStation { get; set; }

        private City(ApplicationContext context, NetHub hub)
        {
            _hub = hub;
            _context = context;

            SubStation = new Station(_context, _hub);
            NuclearStation = new NuclearStation(_context, _hub); 

            SubStation.Start();
            NuclearStation.Start();
        }
        

        public static City Create(ApplicationContext context, NetHub hub)
        {
            return _instance = new City(context, hub);
        }

        public static City GetInstance()
        {
            if (_instance == null)
                throw new System.Exception($"Необходимо использовать метод {nameof(City.Create)} для создания экземпляра класса");
            return _instance;
        }
    }
}
