using LearnHub.Interfaces;
using LearnHub.Models;
using LearnHub.Models.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NuGet.Versioning;
using System.Diagnostics;

namespace LearnHub.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly LearnHubContext _context;
        private readonly IUserService _userService;

        public HomeController(ILogger<HomeController> logger, LearnHubContext context, IUserService userService)
        {
            _logger = logger;
            _context = context;
            _userService = userService;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            /*var currentUser = HttpContext.Session.GetString("CurrentUser");
            var user = await _userService.GetUserByUsernameAsync(currentUser);

            var lastEnrollment = _context.Enrollments
                .Where(u => u.UserId == user.UserId)
                .OrderByDescending(u => u.EnrollmentDate)
                .FirstOrDefault();*/

            var courses = _context.Courses
                .Include(u => u.User)
                .Include(g => g.Grades)
                .Include(e => e.Enrollments)
                .ToList();
            return View(courses);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
