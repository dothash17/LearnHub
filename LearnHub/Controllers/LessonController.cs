using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using LearnHub.Models;
using LearnHub.Models.Data;
using Newtonsoft.Json;

namespace LearnHub.Controllers
{
    public class LessonController : Controller
    {
        private readonly LearnHubContext _context;

        public LessonController(LearnHubContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var learnHubContext = _context.Lessons.Include(l => l.Course);
            return View(await learnHubContext.ToListAsync());
        }

        [HttpGet]
        public async Task<IActionResult> Details(int id)
        {
            var lesson = await _context.Lessons.Include(l => l.Assignments).FirstOrDefaultAsync(c => c.LessonId == id);
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
        public IActionResult GetLessonContent(int id)
        {
            var lesson = _context.Lessons.Include(l => l.Assignments).FirstOrDefault(l => l.LessonId == id);
            var lessonContent = new
            {
                text = lesson.Text,
                assignments = lesson.Assignments.Select(a => new { task = a.Task, answer = a.Answer }).ToList()
            };
            return Ok(lessonContent);
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
