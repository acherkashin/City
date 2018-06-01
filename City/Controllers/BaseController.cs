using System.Linq;
using CyberCity.Models;
using CyberCity.Models.AccountModel;
using CyberCity.Utils;
using Microsoft.AspNetCore.Mvc;

namespace CyberCity.Controllers
{
    public class BaseController : Controller
    {
        protected ApplicationContext _context;

        protected User GetCurrentUser()
        {
            var id = HttpContext.User.GetId();
            var user = _context.Users.FirstOrDefault(u => u.Id == id);

            return user;
        }
    }
}
