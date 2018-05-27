using System;

namespace CyberCity.Models.WeatherStantionModel
{
    public class WeatherStantion: ICityObject
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
        /// Контекст данных.
        /// </summary>
        private ApplicationContext _context;

        /// <summary>
        /// Шина передачи пакетов
        /// </summary>
        private DataBus _bus;

        /// <summary>
        /// Конструктор класса
        /// </summary>
        /// <param name="context"> Контекст данных.</param>
        /// <param name="bus"> Шина передачи пакетов.</param>
        public WeatherStantion(ApplicationContext context, DataBus bus)
        {
            _context = context;
            _bus = bus;
        }

        /// <summary>
        /// Установить режим работы.
        /// </summary>
        /// <param name="mode"> Режим работы.</param>
        public void SetPowerMode(bool mode)
        {
            City.GetInstance().WeatherStantion.IsOn = mode;            
        }

        /// <summary>
        /// Обработать пакет.
        /// </summary>
        /// <param name="package"> Обрабатываемый пакет.</param>
        public void ProcessPackage(Package package)
        {
            var response = new Package();

            response.From = package.To;
            response.To = package.From; 

            var request = package.Method;

            var method = "";
            var parameter = "";
            if (request.Contains("?"))
            {
                method = request.Substring(0, request.IndexOf('?')); // Получить имя метода. 
                parameter = request.Substring(request.IndexOf('?') + 1); // Получить список параметров
            }
            else
            {
                method = request;
            }

            switch (method)
            {
                case "GetAmbience": // Получить летную обстановку.
                    var result = GetAmbience();
                    response.Method = "AllowFlight?" + result.ToString();
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
    }
}
