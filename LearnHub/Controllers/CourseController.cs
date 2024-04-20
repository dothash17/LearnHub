using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using LearnHub.Models;
using LearnHub.Models.Data;
using LearnHub.Interfaces;

namespace LearnHub.Controllers
{
    public class CourseController : Controller
    {
        private readonly LearnHubContext _context;
        private readonly IUserService _userService;

        public CourseController(LearnHubContext context, IUserService userService)
        {
            _context = context;
            _userService = userService;
        }

        public IActionResult Index()
        {
            var courses = _context.Courses
                .Include(u => u.User)
                .Include(g => g.Grades)
                .Include(e => e.Enrollments)
                .ToList();
            return View(courses);
        }

        [HttpGet]
        public async Task<IActionResult> Teaching()
        {
            var currentUser = HttpContext.Session.GetString("CurrentUser");
            var user = await _userService.GetUserByUsernameAsync(currentUser);
            var courses = _context.Courses.Where(s => s.UserId == user.UserId).ToList();
            return View(courses);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Courses courses)
        {
            var currentUser = HttpContext.Session.GetString("CurrentUser");
            var user = await _userService.GetUserByUsernameAsync(currentUser);
            courses.UserId = user.UserId;

            _context.Add(courses);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Teaching));
        }

        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            var course = await _context.Courses.FindAsync(id);
            _context.Courses.Remove(course);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Teaching));
        }

        [HttpGet]
        public async Task<IActionResult> Details(int id)
        {
            var course = await _context.Courses.Include(l => l.Lessons).FirstOrDefaultAsync(c => c.CourseId == id);
            return View(course);
        }

        [HttpGet]
        public async Task<IActionResult> Info(int id)
        {
            var course = await _context.Courses
                .Include(l => l.Lessons)
                .Include(e => e.Enrollments)
                .FirstOrDefaultAsync(c => c.CourseId == id);
            return View(course);
        }

        [HttpPost]
        public async Task<IActionResult> Info(Courses courses)
        {
            var currentUser = HttpContext.Session.GetString("CurrentUser");
            var user = await _userService.GetUserByUsernameAsync(currentUser);
            var enrollment = new Enrollments
            {
                UserId = user.UserId,
                CourseId = courses.CourseId,
                EnrollmentDate = DateTime.UtcNow,
            };
            _context.Enrollments.Add(enrollment);
            await _context.SaveChangesAsync();
            return RedirectToAction("Passage", "Lesson", new {id = courses.CourseId});
        }
    }
}
