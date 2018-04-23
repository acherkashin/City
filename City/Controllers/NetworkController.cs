using City.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace City.Controllers
{
    public class NetworkController : Controller
    {
        private static IHubContext<NetHub> _hubcontext;
        private ApplicationContext _context;

        public NetworkController(ApplicationContext context, IHubContext<NetHub> hubcontext)
        {
            _context = context;
            _hubcontext = hubcontext;
        }

        public void Send(Package package)
        {
            _context.Add(package);
            _context.SaveChanges();
            _hubcontext.Clients.All.SendAsync("Send", package);
        }
    }
}
