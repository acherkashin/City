using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace CyberCity.Models
{
    /// <summary>
    /// Шина данных
    /// </summary>
    public class DataBus
    {
        //var optionsBuilder = new DbContextOptionsBuilder<ApplicationContext>();
        //optionsBuilder.UseSqlite("Data Source=city.db");
        //new ApplicationContext(optionsBuilder.Options);

        private object lockObj = new object();
        private readonly ApplicationContext _context;
        private readonly IHubContext<NetHub, INetHub> _hubContext;

        public DataBus(ApplicationContext context, IHubContext<NetHub, INetHub> hubContext)
        {
            _context = context;
            _hubContext = hubContext;
        }

        public void Send(Package package)
        {
            lock (lockObj)
            {
                _context.Add(package);
                _context.SaveChanges();

                var encrepted = package.CreateEncreted();

                _hubContext.Clients.Group(Subject.Hacker.ToString()).onRecievePackage(package);

                _hubContext.Clients.Group(package.To.ToString()).onRecievePackage(package);

                City.GetInstance().GetObject(package.To)?.ProcessPackage(package);
            }
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

        public string SendToArduino(string url)
        {
            try
            {
                var retuest = WebRequest.Create(url);

                retuest.Method = "POST";
                retuest.ContentType = "application/x-www-urlencoded";
                retuest.Timeout = 5000;

                WebResponse deviceResponse = retuest.GetResponse();

                var deviceAnswer = string.Empty;

                using (var stream = deviceResponse.GetResponseStream())
                {
                    using (var reader = new StreamReader(stream))
                    {
                        deviceAnswer = reader.ReadToEnd();
                    }
                }

                return deviceAnswer;
            }
            catch (Exception ex)
            {
                return null;
            }
        }
    }
}
