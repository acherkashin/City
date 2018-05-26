using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CyberCity.Models
{
    /// <summary>
    /// Шина данных
    /// </summary>
    public class DataBus
    {
        private readonly ApplicationContext _context;
        private readonly IHubContext<NetHub, INetHub> _hubContext;

        public DataBus(ApplicationContext context, IHubContext<NetHub, INetHub> hubContext)
        {
            _context = context;
            _hubContext = hubContext;
        }

        public void Send(Package package)
        {
            _context.Add(package);
            _context.SaveChanges();

            var encrepted = package.CreateEncreted();

            _hubContext.Clients.Group(Subject.Hacker.ToString()).onRecievePackage(package);

            _hubContext.Clients.Group(package.To.ToString()).onRecievePackage(package);

            City.GetInstance().GetObject(package.To).ProcessPackage(package);
        }

        public void SendStateChanged(Subject subject, object state)
        {
            try
            {
                _hubContext.Clients.Group(subject.ToString()).onStateChanged(state);
            }
            catch (Exception ex)
            {
            }
        }
    }
}
