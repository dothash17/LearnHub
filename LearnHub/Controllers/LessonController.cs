using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using LearnHub.Models;
using LearnHub.Models.Data;
using Newtonsoft.Json;
using LearnHub.Interfaces;
using System.Net.WebSockets;

namespace LearnHub.Controllers
{
    public class LessonController : Controller
    {
        private readonly LearnHubContext _context;
        private readonly IUserService _userService;

        public LessonController(LearnHubContext context, IUserService userService)
        {
            _context = context;
            _userService = userService;
        }

        public async Task<IActionResult> Index()
        {
            var learnHubContext = _context.Lessons.Include(l => l.Course);
            return View(await learnHubContext.ToListAsync());
        }

        [HttpGet]
        public async Task<IActionResult> Details(int id)
        {
            var lesson = await _context.Lessons
                .Include(l => l.Assignments)
                .Include(l => l.Course)
                .FirstOrDefaultAsync(c => c.LessonId == id);
            ViewBag.CourseOnDraft = lesson.Course.Status == "Draft";
            return View(lesson);
        }

        [HttpGet]
        public async Task<IActionResult> Passage(int id)
        {
            var lessons = await _context.Lessons
                .Where(c => c.CourseId == id)
                .Include(l => l.Assignments)
                .ToListAsync();
            return View(lessons);
        }

        [HttpGet]
        public async Task<IActionResult> GetLessonContent(int id)
        {
            var currentUser = HttpContext.Session.GetString("CurrentUser");
            var user = await _userService.GetUserByUsernameAsync(currentUser);

            var lesson = _context.Lessons.Include(l => l.Assignments).FirstOrDefault(l => l.LessonId == id);
            var userProgress = _context.Progress
                .Where(p => p.UserId == user.UserId && lesson.Assignments.Select(a => a.AssignmentId).Contains(p.AssignmentId))
                .Select(p => p.AssignmentId)
                .ToList();

            var lessonContent = new
            {
                text = lesson.Text,
                assignments = lesson.Assignments.Select(a => new 
                { 
                    id = a.AssignmentId, 
                    task = a.Task, 
                    answer = a.Answer,
                    solved = userProgress.Contains(a.AssignmentId)
                }).ToList()
            };
            return Ok(lessonContent);
        }

        [HttpPost]
        public async Task<IActionResult> RecordProgress(int assignmentId)
        {
            var currentUser = HttpContext.Session.GetString("CurrentUser");
            var user = await _userService.GetUserByUsernameAsync(currentUser);

            var progress = new Progress
            {
                CompletedAssignment = DateTime.UtcNow,
                AssignmentId = assignmentId,
                UserId = user.UserId
            };

            _context.Add(progress);
            await _context.SaveChangesAsync();

            return Ok();
        }

        [HttpGet]
        public IActionResult Create(int courseId)
        {
            ViewBag.CourseId = courseId;
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Lessons lessons)
        {
            _context.Add(lessons);
            await _context.SaveChangesAsync();
            return RedirectToAction("Details", "Course", new { id = lessons.CourseId});
        }

        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            var lesson = await _context.Lessons.FindAsync(id);
            _context.Lessons.Remove(lesson);
            await _context.SaveChangesAsync();
            return RedirectToAction("Details", "Course", new { id = lesson.CourseId});
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var lesson = await _context.Lessons.FirstOrDefaultAsync(c => c.LessonId == id);
            return View(lesson);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(Lessons lesson)
        {
            _context.Lessons.Update(lesson);
            await _context.SaveChangesAsync();
            return RedirectToAction("Details", "Course", new { id = lesson.CourseId });
        }
    }
}
