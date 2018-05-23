using CyberCity.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CyberCity.Controllers
{
    [Route("api/[controller]")]
    public class NetworkController : Controller
    {
        private ApplicationContext _context;

        public NetworkController(ApplicationContext context)
        {
            _context = context;
        }

        [HttpGet("packages")]
        public ActionResult GetPackages(ApplicationContext context, Subject subject)
        {
            if (subject.Equals(Subject.Admin))
            {
                return Ok(_context.Packages.ToList());
            }

            return Ok(_context.Packages.Where(package => package.To.Equals(subject)).ToList());
        }
    }
}
