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
            ViewBag.Mode = "false";

            if (!User.Identity.IsAuthenticated)
                return Redirect("/Account/Login");

            if (User.IsInRole("Admin"))
                return Redirect("/Account/Admin");


            if (User.IsInRole(Subject.Hacker.ToString()))
                return View("Views/Hacker/Index.cshtml");

            if (User.IsInRole(Subject.Airport.ToString()))
                return Redirect("Airport");

            if (User.IsInRole(Subject.Municipality.ToString()))
                return Redirect("Municipality");

            if (User.IsInRole(Subject.Bank.ToString()))
                return Redirect("Bank");

            if (User.IsInRole(Subject.Houses.ToString()))
                return Redirect("House");

            if (User.IsInRole(Subject.Substation.ToString()))
                return Redirect("Substation");

            if (User.IsInRole(Subject.WeatherStation.ToString()))
                return Redirect("Weather");

            if (User.IsInRole(Subject.NuclearStation.ToString()))
                return Redirect("NuclearStation");

            ViewBag.ArduinoUrl = GetCurrentUser().ArduinoUrl;

            return View();
        }

        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
