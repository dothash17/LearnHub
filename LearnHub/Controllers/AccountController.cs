using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using LearnHub.Models;
using LearnHub.Interfaces;

namespace LearnHub.Controllers
{
    public class AccountController : Controller
    {
        private readonly IUserService _userService;

        public AccountController(IUserService userService)
        {
            _userService = userService;
        }

        [HttpGet]
        public async Task<IActionResult> Profile()
        {
            var currentUser = HttpContext.Session.GetString("CurrentUser");
            var user = await _userService.GetUserByUsernameAsync(currentUser);

            if (user == null)
            {
                return RedirectToAction("Index", "Home");
            }
            return View(user);
        }

        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Register(Users model, string confirmPassword)
        {
            if (string.IsNullOrEmpty(model.Username) || string.IsNullOrEmpty(model.Password))
            {
                return BadRequest("Не все поля заполнены");
            }

            var existingUser = await _userService.GetUserByUsernameAsync(model.Username);
            if (existingUser != null)
            {
                return BadRequest("Пользователь с таким именем уже существует.");
            }

            existingUser = await _userService.GetUserByEmailAsync(model.Mail);
            if (existingUser != null)
            {
                return BadRequest("Email уже используется другим пользователем.");
            }

            if (model.Password != confirmPassword)
            {
                return BadRequest("Пароли не совпадают");
            }

            await _userService.CreateUserAsync(model);

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, model.Username)
            };

            var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var principal = new ClaimsPrincipal(identity);

            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);
            HttpContext.Session.SetString("CurrentUser", model.Username);

            return Ok();
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(Users model)
        {
            if (string.IsNullOrEmpty(model.Username) || string.IsNullOrEmpty(model.Password))
            {
                return BadRequest("Не все поля заполнены");
            }

            var user = await _userService.GetUserByUsernameAsync(model.Username);

            if (user == null)
            {
                return BadRequest("Пользователя с таким именем не существует.");
            }

            if (user.Password != model.Password)
            {
                return BadRequest("Пароль неверный.");
            }

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, user.Username)
            };

            var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var principal = new ClaimsPrincipal(identity);

            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);
            HttpContext.Session.SetString("CurrentUser", user.Username);

            return Ok();
        }
        
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            HttpContext.Session.Remove("CurrentUser");
            return RedirectToAction("Index", "Home");
        }
    }
}