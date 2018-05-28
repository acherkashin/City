using CyberCity.Models;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CyberCity.Controllers
{
    [Route("api/[controller]")]
    public class NuclearStationController : Controller
    {
        [HttpPut("rod")]
        public void ChangeStateRele([FromQuery]bool flag)
        {
            CyberCity.Models.City.GetInstance().NuclearStation.ChangeRodState(flag);
        }

        [HttpGet("state")]
        public ActionResult GetState()
        {
            return Ok(CyberCity.Models.City.GetInstance().NuclearStation.GetState());
        }
    }
}
