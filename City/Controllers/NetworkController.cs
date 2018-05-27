using CyberCity.Models;
using CyberCity.Utils;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CyberCity.Controllers
{
    [Route("api/[controller]")]
    public class NetworkController : BaseController
    {
        private DataBus _bus;

        public NetworkController(ApplicationContext context, DataBus bus)
        {
            _context = context;
            _bus = bus;
        }

        [HttpGet("packages")]
        public ActionResult GetPackages()
        {
            var role = User.GetRole();

            if (role.Equals(Subject.Admin.ToString()))
            {
                return Ok(_context.Packages.ToList());
            }

            var packages = _context.Packages.Where(package => package.To.ToString().Equals(role)).ToList();

            if (role.Equals(Subject.Hacker.ToString()))
            {
                return Ok(packages.ToList());
            }
            else
            {
                return Ok(packages.Select(p => p.CreateEncreted()).ToList());
            }
        }

        [HttpPost("send-package")]
        public ActionResult SendPackage([FromBody]Package package)
        {
            try
            {
                _bus.Send(package);
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }
        }
    }
}
