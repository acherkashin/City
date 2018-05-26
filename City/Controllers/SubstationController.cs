using CyberCity.Models;
using Microsoft.AspNetCore.Mvc;

namespace CyberCity.Controllers
{
    [Route("api/[controller]")]
    public class SubstationController : Controller
    {
        private readonly City _city;

        public SubstationController(City city)
        {
            _city = city;
        }

        [HttpPut("rele")]
        public void ChangeStateRele([FromQuery]bool flag)
        {
            _city.SubStation.ChangeRele(flag);
        }

        [HttpGet("state")]
        public ActionResult GetState()
        {
            return Ok(_city.SubStation.GetState());
        }
    }
}
