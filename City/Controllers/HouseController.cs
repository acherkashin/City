using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using City.Models;
using City.Models.HouseModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace City.Controllers
{
    public class HouseController : Controller
    {
        private IHouseRepository _houseRepository;

        public HouseController(IHouseRepository houseRepository)
        {
            _houseRepository = houseRepository;
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
            return _houseRepository.GetAll();
        }

        /// <summary>
        /// Получение дома по id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public IActionResult GetById(int id)
        {
            var item = _houseRepository.Find(id);
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
            var homes = _houseRepository.GetAll();
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

            return new JsonResult(package);
        }

        /// <summary>
        /// Формирования пакета запроса данных о можности 
        /// </summary>
        /// <returns></returns>
        public JsonResult GetPowerPackage()
        {
            var package = new Package()
            {
                From = Subject.Houses,
                To = Subject.ElectricalSubstation,
                Method = "GetPower",
                Params = ""
            };

            return new JsonResult(package);
        }

        public JsonResult SwitchLight(int countHouses)
        {
            var package = new Package()
            {
                From = Subject.Houses,
                To = Subject.ElectricalSubstation,
                Method = "SwitchLight",
                Params = ""
            };
            return new JsonResult(package);

        }
    }
}