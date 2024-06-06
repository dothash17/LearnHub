using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using LearnHub.Models;
using LearnHub.Interfaces;
using Microsoft.EntityFrameworkCore;
using LearnHub.Models.Data;

namespace LearnHub.Controllers
{
    public class AccountController : Controller
    {
        private readonly LearnHubContext _context;
        private readonly IUserService _userService;

        public AccountController(LearnHubContext context, IUserService userService)
        {
            _context = context;
            _userService = userService;
        }

        [HttpGet]
        public async Task<IActionResult> Profile()
        {
            var currentUser = HttpContext.Session.GetString("CurrentUser");
            var user = await _userService.GetUserByUsernameAsync(currentUser);

            var solvedTasksCount = await _context.Progress.CountAsync(p => p.UserId == user.UserId);
            var completedCoursesCount = await _context.Courses
                .Where(c => c.Lessons.Any(l => l.Assignments.Any()) &&
                    c.Lessons.All(l => l.Assignments.All(a => a.Progress.Any(p => p.UserId == user.UserId))))
                .CountAsync();

            var activityData = await _context.Progress
                .Where(p => p.UserId == user.UserId)
                .GroupBy(p => p.CompletedAssignment.Date)
                .Select(g => new { Date = g.Key, Count = g.Count() })
                .ToDictionaryAsync(g => g.Date.ToString("yyyy-MM-dd"), g => g.Count);

            ViewBag.SolvedTasksCount = solvedTasksCount;
            ViewBag.CompletedCoursesCount = completedCoursesCount;
            ViewBag.ActivityData = activityData;

            return View(user);
        }       

        [HttpGet]
        public async Task<IActionResult> OtherProfile(int userId)
        {
            var currentUser = HttpContext.Session.GetString("CurrentUser");
            var user = await _userService.GetUserByUsernameAsync(currentUser);

            if (user.UserId == userId)
            {
                return RedirectToAction("Profile");
            }

            var otherUser = await _context.Users.FirstOrDefaultAsync(u => u.UserId == userId);
            return View(otherUser);
        }

        [HttpGet]
        public async Task<IActionResult> Message(int? userId = null)
        {
            var currentUser = HttpContext.Session.GetString("CurrentUser");
            var user = await _userService.GetUserByUsernameAsync(currentUser);

            var userChats = await _context.Chats
                .Include(c => c.Messages)
                .Include(c => c.FirstParticipantNavigation)
                .Include(c => c.SecondParticipantNavigation)
                .Where(c => c.FirstParticipant == user.UserId || c.SecondParticipant == user.UserId)
                .ToListAsync();

            userChats = userChats.OrderByDescending(c => c.Messages.Any() ? c.Messages.Max(m => m.SentDate) : DateTime.MinValue).ToList();

            ViewBag.CurrentUserId = user.UserId;
            ViewBag.UserChats = userChats;

            if (userId == null)
            {
                return View();
            }

            var recipientUser = await _userService.GetUserByIdAsync(userId.Value);
            var chat = await _context.Chats
                .Include(c => c.Messages)
                .FirstOrDefaultAsync(c =>
                    (c.FirstParticipant == user.UserId && c.SecondParticipant == recipientUser.UserId) ||
                    (c.FirstParticipant == recipientUser.UserId && c.SecondParticipant == user.UserId));

            if (chat == null)
            {
                chat = new Chats
                {
                    FirstParticipant = user.UserId,
                    SecondParticipant = recipientUser.UserId,
                };
                _context.Chats.Add(chat);
                await _context.SaveChangesAsync();
            }

            ViewBag.RecipientAvatar = recipientUser.Avatar;
            ViewBag.RecipientUsername = recipientUser.Username;
            ViewBag.LastMessage = chat.Messages.Any() ? chat.Messages.OrderByDescending(d => d.SentDate).First() : null;

            return View(chat);
        }

        [HttpPost]
        public async Task<IActionResult> SendMessage(int chatId, string messageText)
        {
            var currentUser = HttpContext.Session.GetString("CurrentUser");
            var user = await _userService.GetUserByUsernameAsync(currentUser);
            var chat = await _context.Chats.Include(c => c.Messages).FirstOrDefaultAsync(c => c.ChatId == chatId);

            var message = new Messages
            {
                ChatId = chatId,
                MessageText = messageText.Trim(),
                SentDate = DateTime.Now,
                SenderId = user.UserId,
                RecipientId = chat.FirstParticipant == user.UserId ? chat.SecondParticipant : chat.FirstParticipant
            };

            _context.Messages.Add(message);
            await _context.SaveChangesAsync();

            return RedirectToAction("Message", new { userId = message.RecipientId });
        }

        [HttpGet]
        public async Task<IActionResult> Settings()
        {
            var currentUser = HttpContext.Session.GetString("CurrentUser");
            var user = await _userService.GetUserByUsernameAsync(currentUser);
            return View(user);
        }

        [HttpPost]
        public async Task<IActionResult> Settings(Users user, IFormFile AvatarFile)
        {
            var currentUser = HttpContext.Session.GetString("CurrentUser");
            var existingUser = await _userService.GetUserByUsernameAsync(currentUser);

            existingUser.LastName = user.LastName;
            existingUser.FirstName = user.FirstName;
            existingUser.Email = user.Email;

            if (AvatarFile != null && AvatarFile.Length > 0)
            {
                var uploadsDirectory = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads", "avatars");
                if (!Directory.Exists(uploadsDirectory))
                {
                    Directory.CreateDirectory(uploadsDirectory);
                }

                var fileName = Guid.NewGuid().ToString() + Path.GetExtension(AvatarFile.FileName);
                var filePath = Path.Combine(uploadsDirectory, fileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await AvatarFile.CopyToAsync(stream);
                }

                existingUser.Avatar = "/uploads/avatars/" + fileName; 
            }

            await _userService.UpdateUserAsync(existingUser);
            return RedirectToAction("Profile");
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

            existingUser = await _userService.GetUserByEmailAsync(model.Email);
            if (existingUser != null)
            {
                return BadRequest("Email уже используется другим пользователем.");
            }

            if (model.Password != confirmPassword)
            {
                return BadRequest("Пароли не совпадают");
            }

            model.Password = BCrypt.Net.BCrypt.HashPassword(model.Password);
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

            if (!BCrypt.Net.BCrypt.Verify(model.Password, user.Password))
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