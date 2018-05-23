using CyberCity.Models.ReactorModel;
using CyberCity.Models.SubStation;
using Microsoft.AspNetCore.SignalR;

namespace CyberCity.Models
{
    public class City
    {
        private static City _instance;
        /// <summary>
        /// Метод для получения экземпляра города(синглтона). Необходим для того чтобы иметь возможность использовать город в любом месте программы
        /// и не ограничиваться возможностями внедрения через констрктор asp.net core.
        /// </summary>
        public static City GetInstance()
        {
            return _instance;
        }

        private DataBus _databus;
        private ApplicationContext _context;

        public Station SubStation { get; private set; }
        public NuclearStation NuclearStation { get; private set; }

        public City(ApplicationContext context, DataBus databus)
        {
            _databus = databus;
            _context = context;

            SubStation = new Station(_context, databus);
            NuclearStation = new NuclearStation(_context, databus);

            SubStation.Start();
            NuclearStation.Start();

            _instance = this;
        }

        public ICityObject GetObject(Subject subj)
        {
            switch (subj)
            {
                case Subject.NuclearStation: return NuclearStation;
                case Subject.Substation: return SubStation;
            }

            return null;
        }
    }
}
