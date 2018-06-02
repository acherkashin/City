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
        /// Получение всех домов
        /// </summary>
        /// <returns></returns>
        public IEnumerable<House> GetAll()
        {
            return _city.Houses.Homes;
        }

        /// <summary>
        /// Получение дома по id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public IActionResult GetById(int id)
        {
            var item = _city.Houses.Homes.Find(x=>x.Id == id);
            if (item == null)
            {
                return NotFound();
            }
            return new ObjectResult(item);
        }

        [HttpPost]
        public void HandleSwitchLight(int id, bool isOnLight)
        {
            var house = _city.Houses.Homes.Find(x=>x.Id == id);

                house.IsOnLight = isOnLight;
                _city.Houses.SwitchLightOnArduino();
        }
    }
}