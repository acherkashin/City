using System;
using Microsoft.AspNetCore.SignalR;
using System.Linq;
using System.Threading.Tasks;
using City.Models;

namespace City
{
    public class NetHub : Hub
    {
        public async Task Send(string message)
        {
            await Clients.All.SendAsync(nameof(Send), message);
        }

        public async Task Register(Subject subject)
        {
            await Clients.All.SendAsync(nameof(Register), subject);
        }
    }
}