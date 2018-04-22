using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using City.Models;
using Microsoft.AspNetCore.Authorization;

namespace City.Controllers
{
    public class HomeController : Controller
    {
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

            return View();
        }

        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

    }
}
