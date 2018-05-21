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
        Task onRecieve(Package package);
        Task onUpdateOnlineList(IEnumerable<User> users);
        void onStateChanged(object state);
    }

    public class NetHub : Hub<INetHub>
    {
        public static List<int> OnlineUsersIds = new List<int>();

        private ApplicationContext _context;

        private Models.City _city;

        //private ReactorRunner _reactor;

        public NetHub(ApplicationContext context)
        {
            _context = context;
            _city = CyberCity.Models.City.Create(this);
            //_reactor = new ReactorRunner(context);
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

            await UpdateOnlineUserList();
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
            await UpdateOnlineUserList();
            await base.OnDisconnectedAsync(exception);
        }

        public async Task Send(Package package)
        {
            _context.Add(package);
            _context.SaveChanges();

            var encrepted = package.CreateEncreted();
            await Clients.Group(Subject.Hacker.ToString()).onRecieve(package);

            await Clients.Group(package.To.ToString()).onRecieve(package);
        }

        private async Task UpdateOnlineUserList()
        {
            var onlineUsers = _context.Users.Where(u => OnlineUsersIds.Contains(u.Id) && u.Subject != Subject.Admin).ToList();
            await Clients.Group(Subject.Admin.ToString()).onUpdateOnlineList(onlineUsers);
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