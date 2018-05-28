using CyberCity.Models.BankModel;
using CyberCity.Models.MunicipalityModel;
using CyberCity.Models.ReactorModel;
using CyberCity.Models.SubStationModel;
using CyberCity.Models.WeatherStantionModel;
using System;
using CyberCity.Models.HouseModels;

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
        public readonly Municipality Municipality;
        public readonly Bank Bank;
        public readonly HouseRepository Houses;


        public City(ApplicationContext context, DataBus databus)
        {
            _databus = databus;
            _context = context;

            SubStation = new SubStation(_context, _databus);
            NuclearStation = new NuclearStation(_context, _databus);
            WeatherStantion = new WeatherStantion(_context, _databus);
            Municipality = new Municipality(_context, _databus);
            Bank = new Bank(_context, _databus);
            Houses = new HouseRepository(_context, _databus);

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
                case Subject.Houses: return Houses;
                    //default: throw new ArgumentException($"Неизвестный тип объекта: ${subj.ToString()}");
            }

            return null;
        }
    }
}
