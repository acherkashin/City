using Microsoft.AspNetCore.SignalR;
using System.Threading.Tasks;
using City.Models;

namespace City
{
    public interface INetHub
    {
        Task Send(Package package);

        Task Register(Subject subject);
    }

    public class NetHub : Hub<INetHub>
    {
        public async Task Send(Package package)
        {
            await Clients.All.Send(package);
        }

        public async Task Register(Subject subject)
        {
            await Clients.All.Register(subject);
        }
    }
}