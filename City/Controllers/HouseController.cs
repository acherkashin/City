using System.Collections.Generic;
using System.Net;
using CyberCity.Models;
using CyberCity.Models.HouseModels;
using CyberCity.Models.SubStationModel;
using Microsoft.AspNetCore.Mvc;

namespace CyberCity.Controllers
{
    public class HouseController : Controller
    {
        //private IHouseRepository _houseRepository;
        //todo почему-то при каждом обновлении страницы вызывается конструктор
        //public HouseController(IHouseRepository houseRepository)
        //{
        //    _houseRepository = houseRepository;
        //}

        public IActionResult Index()
        {
            if (!User.Identity.IsAuthenticated)
            {
                return Redirect("/Account/Login");
            }
            return View();
        }

        /// <summary>
        /// Обновления данных о состоянии жилых домов
        /// </summary>
        public ActionResult UpadateData()
        {
           var houses = HouseRepository.GetAll();
            foreach (var house in houses)
            {
                house.UpdateMeters();
            }

            return StatusCode(200);
        }

        /// <summary>
        /// Получение всех домов
        /// </summary>
        /// <returns></returns>
        public IEnumerable<House> GetAll()
        {
            return HouseRepository.GetAll();
        }

        /// <summary>
        /// Получение дома по id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public IActionResult GetById(int id)
        {
            var item = HouseRepository.Find(id);
            if (item == null)
            {
                return NotFound();
            }
            return new ObjectResult(item);
        }

        /// <summary>
        /// Формирование пакета с показателями потребляемых ресурсов
        /// </summary>
        /// <returns></returns>
        public JsonResult GetMetersPackage()
        {
            var homes = HouseRepository.GetAll();
            var homeMeters = new List<HouseMeter>();

            foreach (var home in homes)
            {
                float meters = home.GasMeter.CurrentVolume * home.GasMeter.Tarif +
                               home.ElectricMeter.SpentPower * home.ElectricMeter.Tarif +
                               home.WaterMeter.CurrentVolume * home.WaterMeter.Tarif;
                homeMeters.Add(new HouseMeter { IdHome = home.Id, Meters = meters });
            }

            var package = new Package()
            {
                From = Subject.Houses,
                To = Subject.Bank,
                Method = "SendMeters",
                Params = new JsonResult(homeMeters).ToString()
            };

            return new JsonResult(package);
        }


        /// <summary>
        /// Формирование пакета запроса тарифных показателей у Муниципалитета
        /// </summary>
        /// <returns></returns>
        public JsonResult GetTarifPackage()
        {
            var package = new Package()
            {
                From = Subject.Houses,
                To = Subject.Municipality,
                Method = "GetTarif",
                Params = ""
            };

            //todo доделать прием данных от муниципалитета

            return new JsonResult(package);
        }

        /// <summary>
        /// Формирования пакета запроса данных о мощности 
        /// </summary>
        /// <returns></returns>
        public JsonResult GetPowerPackage()
        {
            var package = new Package()
            {
                From = Subject.Houses,
                To = Subject.Substation,
                Method = SubStation.GetPowerMethod,
                Params = ""
            };

            //todo доделать прием данных от подстанции

            var houses = HouseRepository.GetAll();

            int power = Generator.GenerateValue(0, 100);

            //пришло слишком много мощности, поэтому отрубаем свет во всех домах
            if (power > 100)
            {
                foreach (var house in houses)
                {
                    house.IsOnLight = false;
                }
            }
            else if (power <= 100 && power >= 75)
            {
                foreach (var house in houses)
                {
                    house.IsOnLight = true;
                }
            }
            else if (power < 75 && power >= 50)
            {
                HouseRepository.Find(1).IsOnLight = false;

                HouseRepository.Find(2).IsOnLight = true;
                HouseRepository.Find(3).IsOnLight = true;
                HouseRepository.Find(4).IsOnLight = true;

            }
            else if (power < 50 && power >= 25)
            {
                HouseRepository.Find(1).IsOnLight = false;
                HouseRepository.Find(2).IsOnLight = false;

                HouseRepository.Find(3).IsOnLight = true;
                HouseRepository.Find(4).IsOnLight = true;
            }
            else if (power < 25 && power >= 10)
            {
                HouseRepository.Find(1).IsOnLight = false;
                HouseRepository.Find(2).IsOnLight = false;
                HouseRepository.Find(3).IsOnLight = false;

                HouseRepository.Find(4).IsOnLight = true;
            }
            //пришло слишком мало мощности, поэтому отрубаем свет во всех домах
            else if (power < 10 )
            {
                foreach (var house in houses)
                {
                    house.IsOnLight = false;
                }
            }

            return new JsonResult(package);
        }


        public void SwitchLightOnArduino()
        {
            var homes = HouseRepository.GetAll();

            string urlToArduino = "http://192.168.0.0/switchLight?";

            foreach (var home in homes)
            {
               WebRequest request = WebRequest.Create(urlToArduino + $"id=${home.Id}&${home.IsOnLight}");
               request.Method = "GET";
               WebResponse response = request.GetResponse();
            }
        }


        [HttpPost]
        public void HandleSwitchLight(int id, bool isOnLight)
        {
            var house = HouseRepository.Find(id);

            if (house != null)
            {
                house.IsOnLight = isOnLight;
            }
        }
    }
}