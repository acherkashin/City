using CyberCity.Models.AirportModels;
using System;

namespace CyberCity.Models.WeatherStantionModel
{
    /// <summary>
    /// Метеостанция
    /// </summary>
    public class WeatherStantion : CityObject
    {
        /// <summary>
        /// Включена ли станция.
        /// </summary>
        public bool IsOn = true;

        /// <summary>
        /// Генератор случайных чисел. 
        /// </summary>
        private static Random Random = new Random();

        /// <summary>
        /// Число запросов, после которого отправить ответ о неблагоприятных условиях.
        /// </summary>
        public int SendSmash = Random.Next(3, 10);

        /// <summary>
        /// Номер текущего ответа. 
        /// </summary>
        public int CurrentAnswerNumber = 0;

        /// <summary>
        /// Конструктор класса
        /// </summary>
        /// <param name="context"> Контекст данных.</param>
        /// <param name="bus"> Шина передачи пакетов.</param>
        public WeatherStantion(ApplicationContext context, DataBus bus) : base(context, bus) { }

        /// <summary>
        /// Установить режим работы.
        /// </summary>
        /// <param name="mode"> Режим работы.</param>
        public void SetPowerMode(bool mode)
        {
            City.GetInstance().WeatherStantion.IsOn = mode;
            _bus.SendStateChanged(Subject.WeatherStation, GetState());
        }

        /// <summary>
        /// Обработать пакет.
        /// </summary>
        /// <param name="package"> Обрабатываемый пакет.</param>
        public override void ProcessPackage(Package package)
        {
            var response = new Package();

            response.From = package.To;
            response.To = package.From;

            var method = package.Method;
            var parameter = Newtonsoft.Json.JsonConvert.DeserializeObject(package.Params);

            switch (method)
            {
                case Airport.CanFlyMethod: // Получить летную обстановку.
                    response.Method = Airport.CanFlyMethod;
                    response.Params = Newtonsoft.Json.JsonConvert.SerializeObject(GetAmbience());
                    _bus.Send(response);
                    break;
                case Airport.CanLandMethod: // Получить летную обстановку.
                    response.Method = Airport.CanLandMethod;
                    response.Params = Newtonsoft.Json.JsonConvert.SerializeObject(GetAmbience());
                    _bus.Send(response);
                    break;
                case "SetPowerMode": // Установить режим работы.
                    bool mode = true;
                    try
                    {
                        mode = Convert.ToBoolean(parameter);
                    }
                    catch (Exception ex) { }

                    SetPowerMode(mode);
                    break;
            }
        }

        /// <summary>
        /// Получить летную обстановку пункта назначения. 
        /// </summary>
        /// <returns> Можно ли лететь / приземляться.</returns>
        private bool GetAmbience()
        {
            if (SendSmash == CurrentAnswerNumber)
            {
                SendSmash = Random.Next(1, 10);
                CurrentAnswerNumber = 0;

                return false;
            }
            else
            {
                CurrentAnswerNumber++;

                return true;
            }
        }

        /// <summary>
        /// Получить состояние станции.
        /// </summary>
        /// <returns> Состояние.</returns>
        private bool GetState()
        {
            return this.IsOn;
        }
    }
}
