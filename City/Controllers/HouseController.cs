using System.Collections.Generic;
using System.Net;
using CyberCity.Models;
using CyberCity.Models.HouseModels;
using CyberCity.Models.SubStationModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CyberCity.Controllers
{
    [Authorize(Roles = "Houses")]
    public class HouseController : Controller
    {
        private readonly City _city;

        public HouseController(City city)
        {
            _city = city;
        }

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
            var houses = _city.Houses.GetAll();
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
            return _city.Houses.GetAll();
        }

        /// <summary>
        /// Получение дома по id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public IActionResult GetById(int id)
        {
            var item = _city.Houses.Find(id);
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
            var homes = _city.Houses.GetAll();
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


        [HttpPost]
        public void HandleSwitchLight(int id, bool isOnLight)
        {
            var house = _city.Houses.Find(id);

            if (house != null)
            {
                house.IsOnLight = isOnLight;
            }
        }
    }
}