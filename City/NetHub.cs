using Microsoft.AspNetCore.SignalR;
using System.Threading.Tasks;
using System.Linq;
using CyberCity.Models;
using System.Security.Claims;
using System.Collections.Generic;
using System;
using CyberCity.Utils;
using CyberCity.Models.AccountModel;

namespace CyberCity
{
    public interface INetHub
    {
        void onRecievePackage(Package package);
        void onUpdateOnlineList(IEnumerable<User> users);
        void onStateChanged(object state);
    }

    public class NetHub : Hub<INetHub>
    {
        public static List<int> OnlineUsersIds = new List<int>();

        private ApplicationContext _context;


        public NetHub(ApplicationContext context)
        {
            _context = context;
        }

        public async override Task OnConnectedAsync()
        {
            var user = Context.User;
            var userId = GetId(user);

            await Groups.AddAsync(Context.ConnectionId, user.GetRole());

            if (!OnlineUsersIds.Any(id => id == userId))
            {
                OnlineUsersIds.Add(userId);
            }

            UpdateOnlineUserList();
            await base.OnConnectedAsync();
        }

        public async override Task OnDisconnectedAsync(Exception exception)
        {
            OnlineUsersIds.Remove(GetId(Context.User));
            UpdateOnlineUserList();
            await base.OnDisconnectedAsync(exception);
        }

        public void Send(Package package)
        {
            _context.Add(package);
            _context.SaveChanges();

            var encrepted = package.CreateEncreted();

            Clients.Group(Subject.Hacker.ToString()).onRecievePackage(package);

            Clients.Group(package.To.ToString()).onRecievePackage(package);

            CyberCity.Models.City.GetInstance().GetObject(package.To).ProcessPackage(package);
        }

        private void UpdateOnlineUserList()
        {
            var onlineUsers = _context.Users.Where(u => OnlineUsersIds.Contains(u.Id) && u.Subject != Subject.Admin).ToList();
            Clients.Group(Subject.Admin.ToString()).onUpdateOnlineList(onlineUsers);
        }

        private int GetId(ClaimsPrincipal principal)
        {
            var userId = int.Parse(principal.Identities.FirstOrDefault()?.Claims.FirstOrDefault(claim => claim.Type == "ID")?.Value);
            return userId;
        }
    }
}