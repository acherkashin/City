using CyberCity.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CyberCity.Controllers
{
    [Authorize(Roles = "Substation")]
    public class SubstationController : Controller
    {
        private readonly City _city;

        public ActionResult Index()
        {
            return View();
        }

        public SubstationController(City city)
        {
            _city = city;
        }

        [HttpPut("api/[controller]/rele")]
        public void ChangeStateRele([FromQuery]bool flag)
        {
            _city.SubStation.ChangeRele(flag);
        }

        [HttpGet("api/[controller]/state")]
        public ActionResult GetState()
        {
            return Ok(_city.SubStation.GetState());
        }
    }
}
