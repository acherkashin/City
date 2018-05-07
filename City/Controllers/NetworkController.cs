using City.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace City.Controllers
{
    [Route("api/[controller]")]
    public class NetworkController : Controller
    {
        private static IHubContext<NetHub, INetHub> _hubcontext;
        private ApplicationContext _context;

        public NetworkController(ApplicationContext context, IHubContext<NetHub, INetHub> hubcontext)
        {
            _context = context;
            _hubcontext = hubcontext;
        }
    }
}
