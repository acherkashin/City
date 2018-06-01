using CyberCity.Models;
using CyberCity.Models.AccountModel;
using CyberCity.Utils;
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
    public class AccountController : BaseController
    {
        private City _city;

        public AccountController(ApplicationContext context, IHubContext<NetHub> hubcontext, CyberCity.Models.City city)
        {
            _city = city;
            _context = context;
            DatabaseInitialize(); // добавляем пользователя и роли в бд
        }

        private void DatabaseInitialize()
        {
            string adminLogin = "admin";

            if (!_context.Users.Any(user => user.Login == adminLogin))
            {
                string adminPassword = "123456";

                // добавляем администратора
                _context.Users.Add(new User { Login = adminLogin, Password = adminPassword, Subject = Subject.Admin });

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
                    user = model.ToUser();

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

        public async Task<IActionResult> SignOut()
        {
            await HttpContext.SignOutAsync();
            return View(nameof(Login));
        }

        [HttpPut("api/[controller]/update-arduino-url")]
        public async Task<IActionResult> UpdateArduinoUrl([FromBody]UpdateUrlModel model)
        {
            if (ModelState.IsValid)
            {
                var user = GetCurrentUser();
                user.ArduinoUrl = model.ArduinoUrl;


                await _context.SaveChangesAsync();
                return Ok(user);
            }

            return BadRequest();
        }

        private async Task Authenticate(User user)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimsIdentity.DefaultNameClaimType, user.Login),
                new Claim(ClaimsIdentity.DefaultRoleClaimType, user.Subject.ToString()),
                new Claim(UserExtentions.IdClaimType, user.Id.ToString()),
            };

            ClaimsIdentity id = new ClaimsIdentity(claims, "ApplicationCookie", ClaimsIdentity.DefaultNameClaimType,
                ClaimsIdentity.DefaultRoleClaimType);

            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(id));

            if (!user.Subject.Equals(Subject.Admin) && !user.Subject.Equals(Subject.Hacker))
            {
                var cityObject = _city.GetObject(user.Subject);

                if (cityObject != null)
                {
                    cityObject.UserId = user.Id;
                }
            }
        }
    }
}
