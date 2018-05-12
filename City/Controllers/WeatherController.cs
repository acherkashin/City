using System;
using System.IO;
using System.Net;
using City.Models.WeatherStantion;
using City.Statics;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace City.Controllers
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
            if (Statics.StantionPower.IsOn)
            {
                WebRequest request;
                if (city != "PlayCity")
                {
                    request = WebRequest.Create($"http://api.openweathermap.org/data/2.5/weather?q={city}&APPID=48e67cb1b0973a7baa74f011cc36315b");
                }
                else
                {
                    request = WebRequest.Create($"http://api.openweathermap.org/data/2.5/weather?q=Kursk&APPID=48e67cb1b0973a7baa74f011cc36315b");
                }

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


                if (city == "PlayCity")
                {
                    var getRequest = WebRequest.Create($"http://api.openweathermap.org/data/2.5/weather?q=Kursk&APPID=48e67cb1b0973a7baa74f011cc36315b");

                    getRequest.Method = "POST";
                    getRequest.ContentType = "application/x-www-urlencoded";

                    var deviceResponse = getRequest.GetResponse();

                    var deviceAnswer = string.Empty;

                    using (var stream = deviceResponse.GetResponseStream())
                    {
                        using (var reader = new StreamReader(stream))
                        {
                            deviceAnswer = reader.ReadToEnd();
                        }
                    }
                }
                else
                {
                    result.Tempreture = Math.Round(weather.Main.Tempreture, 2);
                    result.Pressure = Math.Round(weather.Main.Pressure, 2);
                    result.Wind = Math.Round(weather.Wind.Speed, 2);
                    result.Icon = weather.Weather[0].Icon;
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
            StantionPower.IsOn = !Statics.StantionPower.IsOn;

            return Statics.StantionPower.IsOn;
        }
    }
}