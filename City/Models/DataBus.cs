using CyberCity.Models.Core;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
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
        private object lockObj = new object();

        public static ApplicationContext Context => ContextFactory.GetContext();
        private readonly IHubContext<NetHub, INetHub> _hubContext;

        public DataBus(IHubContext<NetHub, INetHub> hubContext)
        {
            _hubContext = hubContext;
        }

        public void Send(Package package)
        {
            lock (lockObj)
            {
                Context.Add(package);
                Context.SaveChanges();

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
