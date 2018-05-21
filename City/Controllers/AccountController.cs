using CyberCity.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace CyberCity.Controllers
{
    public class AccountController : Controller
    {
        private ApplicationContext _context;

        public AccountController(ApplicationContext context, IHubContext<NetHub> hubcontext)
        {
            _context = context;
            DatabaseInitialize(); // добавляем пользователя и роли в бд
        }

        private void DatabaseInitialize()
        {
            string adminEmail = "admin";

            if (!_context.Users.Any(user => user.Login == adminEmail))
            {
                string adminPassword = "123456";

                // добавляем администратора
                _context.Users.Add(new User { Login = adminEmail, Password = adminPassword, Subject = Subject.Admin });

                _context.SaveChanges();
            }
        }
        [HttpGet]
        public IActionResult Register()
        {
            if (User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Index", "Home");
            }

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterModel model)
        {
            if (ModelState.IsValid)
            {
                User user = await _context.Users.FirstOrDefaultAsync(u => u.Login == model.Login);
                if (user == null)
                {
                    user = new User { Login = model.Login, Password = model.Password, FirstName = model.FirstName, LastName = model.LastName, Subject = model.Subject };

                    _context.Users.Add(user);
                    await _context.SaveChangesAsync();

                    await Authenticate(user);

                    return RedirectToAction("Index", "Home");
                }
                else
                    ModelState.AddModelError("", "Некорректные логин и(или) пароль");
            }
            return View(model);

        }

        [HttpGet]
        public IActionResult Login()
        {
            if (User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Index", "Home");
            }

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginModel model)
        {
            if (ModelState.IsValid)
            {
                User user = await _context.Users.FirstOrDefaultAsync(u => u.Login == model.Login && u.Password == model.Password);

                if (user != null)
                {
                    await Authenticate(user); // аутентификация

                    return RedirectToAction("Index", "Home");
                }
                ModelState.AddModelError("", "Некорректные логин и(или) пароль");
            }
            return View(model);
        }

        [Authorize(Roles = "Admin")]
        public IActionResult Admin()
        {
            return View(new AdminViewModel
            {
                Users = _context.Users.Where(user => user.Subject != Subject.Admin).ToList(),
            });
        }

        private async Task Authenticate(User user)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimsIdentity.DefaultNameClaimType, user.Login),
                new Claim(ClaimsIdentity.DefaultRoleClaimType, user.Subject.ToString()),
                new Claim("ID", user.Id.ToString()),
            };

            ClaimsIdentity id = new ClaimsIdentity(claims, "ApplicationCookie", ClaimsIdentity.DefaultNameClaimType,
                ClaimsIdentity.DefaultRoleClaimType);

            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(id));
        }
    }
}
