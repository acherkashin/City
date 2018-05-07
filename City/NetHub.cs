using Microsoft.AspNetCore.SignalR;
using System.Threading.Tasks;
using System.Linq;
using City.Models;
using System.Security.Claims;
using System.Collections.Generic;
using System;

namespace City
{
    public interface INetHub
    {
        Task onRecieve(Package package);
        Task onUpdateOnlineList(IEnumerable<User> users);
    }

    public class NetHub : Hub<INetHub>
    {
        public static string AdminRole => Subject.Admin.ToString();
        public static string HackerRole => Subject.Hacker.ToString();
        public static string CityObjectRole => nameof(CityObjectRole);

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

            if (user.IsInRole(AdminRole))
            {
                await Groups.AddAsync(Context.ConnectionId, AdminRole);
            }
            else if (user.IsInRole(HackerRole))
            {
                await Groups.AddAsync(Context.ConnectionId, HackerRole);
            }
            else
            {
                await Groups.AddAsync(Context.ConnectionId, CityObjectRole);
            }

            if (!OnlineUsersIds.Any(id => id == userId))
            {
                OnlineUsersIds.Add(userId);
            }

            await UpdateOnlineUserList();
            await base.OnConnectedAsync();
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

            if (Context.User.IsInRole(HackerRole))
            {
                var encrepted = package.CreateEncreted();
                await Clients.Group(HackerRole).onRecieve(package);
            }
            else
            {
                await Clients.Group(AdminRole).onRecieve(package);
                await Clients.Group(CityObjectRole).onRecieve(package);
            }
        }

        private async Task UpdateOnlineUserList()
        {
            var onlineUsers = _context.Users.Where(u => OnlineUsersIds.Contains(u.Id) && u.Subject != Subject.Admin).ToList();
            await Clients.Group(AdminRole).onUpdateOnlineList(onlineUsers);
        }

        private int GetId(ClaimsPrincipal principal)
        {
            var userId = int.Parse(principal.Identities.FirstOrDefault()?.Claims.FirstOrDefault(claim => claim.Type == "ID")?.Value);
            return userId;
        }
    }
}