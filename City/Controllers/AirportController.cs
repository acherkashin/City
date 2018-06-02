using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using CyberCity.Models.AirportModels;
using CyberCity.Models;
using Microsoft.AspNetCore.Authorization;

namespace CyberCity.Controllers
{
    [Produces("application/json")]
    [Authorize(Roles = "Airport")]
    public class AirportController : Controller
    {

        private readonly CyberCity.Models.City _city;

        public AirportController(CyberCity.Models.City city)
        {
            _city = city;
        }

        public ActionResult Index()
        {
            return View();
        }

        public IActionResult GetAll()
        {
            return new ObjectResult(_city.Airport);
        }
    }
}
