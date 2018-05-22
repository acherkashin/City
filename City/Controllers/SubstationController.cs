using CyberCity.Models;
using CyberCity.Models.SubStation;
using Microsoft.AspNetCore.Mvc;

namespace CyberCity.Controllers
{
    [Route("api/[controller]")]
    public class SubstationController : Controller
    {
        [HttpPut("rele")]
        public void ChangeStateRele([FromQuery]bool flag)
        {
            CyberCity.Models.City.GetInstance().SubStation.ChangeRele(flag);
        }

        [HttpGet("state")]
        public ActionResult GetState()
        {
            return Ok(CyberCity.Models.City.GetInstance().SubStation.GetState());
        }
    }
}
