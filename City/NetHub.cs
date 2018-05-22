using Microsoft.AspNetCore.SignalR;
using System.Threading.Tasks;
using System.Linq;
using CyberCity.Models;
using System.Security.Claims;
using System.Collections.Generic;
using System;
using CyberCity.Models.ReactorModel;

namespace CyberCity
{
    public interface INetHub
    {
        void onRecieve(Package package);
        void onUpdateOnlineList(IEnumerable<User> users);
        void onStateChanged(object state);
    }

    public class NetHub : Hub<INetHub>
    {
        public static List<int> OnlineUsersIds = new List<int>();

        private ApplicationContext _context;

        private City _city;


        public NetHub(ApplicationContext context)
        {
            _context = context;
            _city = City.Create(context, this);
        }

        public async override Task OnConnectedAsync()
        {
            var user = Context.User;
            var userId = GetId(user);

            await Groups.AddAsync(Context.ConnectionId, GetRole(user));

            if (!OnlineUsersIds.Any(id => id == userId))
            {
                OnlineUsersIds.Add(userId);
            }

            UpdateOnlineUserList();
            await base.OnConnectedAsync();
        }

        public void SendStateChanged(Subject subject, object state)
        {
            try
            {
                Clients.Group(subject.ToString()).onStateChanged(state);
            }
            catch (Exception ex)
            {
            }
        }

        public async override Task OnDisconnectedAsync(Exception exception)
        {
            OnlineUsersIds.Remove(GetId(Context.User));
            UpdateOnlineUserList();
            await base.OnDisconnectedAsync(exception);
        }

        public async void Send(Package package)
        {
            _context.Add(package);
            _context.SaveChanges();

            var encrepted = package.CreateEncreted();

            Clients.Group(Subject.Hacker.ToString()).onRecieve(package);

            Clients.Group(package.To.ToString()).onRecieve(package);

            if(package.To.Equals(Subject.Substation))
            {

            }
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

        private string GetRole(ClaimsPrincipal principal)
        {
            return principal.Identities.FirstOrDefault()?.Claims.FirstOrDefault(claim => claim.Type == ClaimsIdentity.DefaultRoleClaimType)?.Value;
        }
    }
}