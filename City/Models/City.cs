using CyberCity.Models.BankModel;
using CyberCity.Models.MunicipalityModel;
using CyberCity.Models.ReactorModel;
using CyberCity.Models.SubStationModel;
using CyberCity.Models.WeatherStantionModel;
using CyberCity.Models.AirportModels;
using System;
using CyberCity.Models.HouseModels;
using System.Threading;
using CyberCity.Models.Core;

namespace CyberCity.Models
{
    public class City
    {
        private static City _instance;

        /// <summary>
        /// Флаг - идет ли время в городе.
        /// </summary>
        public bool IsTimeRunning;

        /// <summary>
        /// Текущий час.
        /// </summary>
        public int Hour;

        /// <summary>
        /// Метод для получения экземпляра города(синглтона). Необходим для того чтобы иметь возможность использовать город в любом месте программы
        /// и не ограничиваться возможностями внедрения через констрктор asp.net core.
        /// </summary>
        public static City GetInstance()
        {
            return _instance;
        }

        private DataBus _databus;

        public readonly SubStation SubStation;
        public readonly NuclearStation NuclearStation;
        public readonly WeatherStantion WeatherStantion;
        public readonly Municipality Municipality;
        public readonly Bank Bank;
        public readonly Houses Houses;
        public readonly Airport Airport;

        private readonly CitySheduler _cityTime = new CitySheduler();


        public City(DataBus databus)
        {
            _databus = databus;

            SubStation = new SubStation(_databus);
            NuclearStation = new NuclearStation(_databus);
            WeatherStantion = new WeatherStantion(_databus);
            Municipality = new Municipality(_databus);
            Bank = new Bank(_databus);
            Houses = new Houses(_databus);
            Airport = new Airport(_databus);

            Start();

            _instance = this;
            IsTimeRunning = true;
        }

        public void Start()
        {
            _cityTime.Start();
            
            _cityTime.RunEach(1, SubStation.Work);
            _cityTime.RunEach(1, NuclearStation.Work);
            
            Municipality.Start();
            Houses.Start();

            _cityTime.RunEach(1, Airport.Work);
        }

        public CityObject GetObject(Subject subj)
        {
            switch (subj)
            {
                case Subject.Substation: return SubStation;
                case Subject.NuclearStation: return NuclearStation;
                case Subject.WeatherStation: return WeatherStantion;
                case Subject.Municipality: return Municipality;
                case Subject.Bank: return Bank;
                case Subject.Houses: return Houses;
                case Subject.Airport: return Airport;

                    //default: throw new ArgumentException($"Неизвестный тип объекта: ${subj.ToString()}");
            }

            return null;
        }


    }
}
