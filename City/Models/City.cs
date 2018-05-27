using CyberCity.Models.ReactorModel;
using CyberCity.Models.SubStationModel;
using CyberCity.Models.WeatherStantionModel;
using System;

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

        public readonly SubStation SubStation;
        public readonly NuclearStation NuclearStation;
        public readonly WeatherStantion WeatherStantion;

        public City(ApplicationContext context, DataBus databus)
        {
            _databus = databus;
            _context = context;

            SubStation = new SubStation(_context, databus);
            NuclearStation = new NuclearStation(_context, databus);
            WeatherStantion = new WeatherStantion(_context, databus);

            SubStation.Start();
            NuclearStation.Start();

            _instance = this;
        }

        public CityObject GetObject(Subject subj)
        {
            switch (subj)
            {
                case Subject.NuclearStation: return NuclearStation;
                case Subject.Substation: return SubStation;
                case Subject.WeatherStation:return WeatherStantion;
                default: throw new ArgumentException($"Неизвестный тип объекта: ${subj.ToString()}");
            }
        }
    }
}
