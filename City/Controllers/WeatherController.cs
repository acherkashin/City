using System;
using System.IO;
using System.Net;
using CyberCity.Models;
using CyberCity.Models.WeatherStantionModel;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace CyberCity.Controllers
{
    public class WeatherController : Controller
    {       
        /// <summary>
        /// 
        /// </summary>
        /// <param name="mode">Режим отображения представления</param>
        /// <returns></returns>        
        public IActionResult Index(int? mode)
        {
            if (mode != null)
            {
                ViewBag.Mode = "true";
            }
            else
            {
                ViewBag.Mode = "false";
            }

            return View();
        }

        /// <summary>
        /// Получить данные о погодных условиях.
        /// </summary>
        /// <param name="city"> Город.</param>
        /// <returns> Метеоусловия.</returns>

        public WeaherResult GetWeather(string city)
        {
            if (City.GetInstance().WeatherStantion.IsOn)
            {
                var requestCity = city;

                if(city == "PlayCity")
                {
                    requestCity = "Kursk";
                }
                
                var request = WebRequest.Create($"http://api.openweathermap.org/data/2.5/weather?q={requestCity}&APPID=48e67cb1b0973a7baa74f011cc36315b");
                
                request.Method = "POST";

                request.ContentType = "application/x-www-urlencoded";

                WebResponse response = request.GetResponse();

                var answer = string.Empty;

                using (var stream = response.GetResponseStream())
                {
                    using (var reader = new StreamReader(stream))
                    {
                        answer = reader.ReadToEnd();
                    }
                }

                response.Close();

                OpenWeather weather = JsonConvert.DeserializeObject<OpenWeather>(answer);

                WeaherResult result = new WeaherResult();

                result.Pressure = Math.Round(weather.Main.Pressure, 2);
                result.Wind = Math.Round(weather.Wind.Speed, 2);
                result.Icon = weather.Weather[0].Icon;


                if (city == "PlayCity")
                {
                    var user = City.GetInstance().WeatherStantion.GetUser();
                    var arduinoUrl = ""; 
                    
                    if(user == null)
                    {
                        arduinoUrl = "192.168.1.6";
                    }
                    else
                    {
                        arduinoUrl = user.ArduinoUrl;
                    }

                    var getRequest = WebRequest.Create($"http://{arduinoUrl}");

                    getRequest.Method = "POST";
                    getRequest.ContentType = "application/x-www-urlencoded";
                    getRequest.Timeout = 5000;

                    WebResponse deviceResponse;
                    try
                    {
                        deviceResponse = getRequest.GetResponse();

                        var deviceAnswer = string.Empty;

                        using (var stream = deviceResponse.GetResponseStream())
                        {
                            using (var reader = new StreamReader(stream))
                            {
                                deviceAnswer = reader.ReadToEnd();
                            }
                        }
                        result.Tempreture = Convert.ToDouble(deviceAnswer.Substring(0, deviceAnswer.IndexOf(":")));
                        result.Humidity = Convert.ToDouble(deviceAnswer.Substring(deviceAnswer.IndexOf(":") + 1));
                    }
                    catch(Exception ex)
                    {
                        result.Tempreture = 0;
                        result.Humidity = 0;
                    }                   
                }
                else
                {
                    result.Tempreture = Math.Round(weather.Main.Tempreture, 2);
                    result.Humidity = Math.Round(weather.Main.Humidity, 2);
                }

                return result;
            }
            else
            {
                return new WeaherResult();
            }
        }

        /// <summary>
        /// Изменить режим работы. 
        /// </summary>
        /// <returns> Включена или нет станция. </returns>               
        public bool ChangePowerMode()
        {
            City.GetInstance().WeatherStantion.IsOn = !City.GetInstance().WeatherStantion.IsOn;

            return City.GetInstance().WeatherStantion.IsOn;
        }

        /// <summary>
        /// Проверка корректности парсинга пакета.
        /// </summary>
        /// <param name="method"> Метод. </param>
        /// <param name="mode"> Передаваемый параметр. </param>
        public void TestPackage(string method, bool mode)
        {
            var package = new Package();
            package.To = Subject.WeatherStation;
            package.From = Subject.Airport;
            package.Method = method;
            package.Params = Newtonsoft.Json.JsonConvert.SerializeObject(mode);

            City.GetInstance().WeatherStantion.ProcessPackage(package);
        }
    }
}