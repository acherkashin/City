using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace City.Controllers
{
    [Produces("application/json")]
    [Route("api/Airport")]
    public class AirportController : Controller
    {

        public ActionResult Index()
        {
            return View();
        }
        // отправить самолет
        public void SendPlane ()
        {

        }

        // посадить самолет
        public void LandPlane()
        {

        }

        //включить свет
        public void TurnOnLight()
        {

        }

        //выключить свет
        public void SwitchOffLight()
        {

        }

        // прием разрешения на вылет/прилет
        public void AllowFlight(bool resolution)
        {

        }
    }
}
