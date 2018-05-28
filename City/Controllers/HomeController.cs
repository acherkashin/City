using CyberCity.Models;
using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using CyberCity.Utils;

namespace CyberCity.Controllers
{
    public class HomeController : BaseController
    {
        public HomeController(ApplicationContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            if (!User.Identity.IsAuthenticated)
            {
                return Redirect("/Account/Login");
            }

            if (User.IsInRole("Admin"))
            {
                return Redirect("/Account/Admin");
            }

            if (User.IsInRole(Subject.Hacker.ToString()))
            {
                return View("Views/Hacker/Index.cshtml");
            }

            if(User.IsInRole(Subject.Bank.ToString()))
            {
                return View("Bank/Bank");
            }

            ViewBag.ArduinoUrl = GetCurrentUser().ArduinoUrl;

            return View();
        }

        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
