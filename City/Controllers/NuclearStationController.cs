using CyberCity.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CyberCity.Controllers
{
    [Authorize(Roles = "NuclearStation")]
    public class NuclearStationController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

        [HttpPut("api/[controller]/rod")]
        public void ChangeStateRele([FromQuery]bool flag)
        {
            City.GetInstance().NuclearStation.ChangeRodState(flag);
        }

        [HttpGet("api/[controller]/state")]
        public ActionResult GetState()
        {
            return Ok(City.GetInstance().NuclearStation.GetState());
        }
    }
}
