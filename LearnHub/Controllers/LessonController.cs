using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using LearnHub.Models;
using LearnHub.Models.Data;

namespace LearnHub.Controllers
{
    public class LessonController : Controller
    {
        private readonly LearnHubContext _context;

        public LessonController(LearnHubContext context)
        {
            _context = context;
        }

        // GET: Lesson
        public async Task<IActionResult> Index()
        {
            var learnHubContext = _context.Lessons.Include(l => l.Course);
            return View(await learnHubContext.ToListAsync());
        }

        // GET: Lesson/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var lessons = await _context.Lessons
                .Include(l => l.Course)
                .FirstOrDefaultAsync(m => m.LessonId == id);
            if (lessons == null)
            {
                return NotFound();
            }

            return View(lessons);
        }

        [HttpGet]
        public async Task<IActionResult> Passage(int id)
        {
            var lessons = await _context.Lessons.Where(c => c.CourseId == id).ToListAsync();
            return View(lessons);
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

        // GET: Lesson/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var lessons = await _context.Lessons.FindAsync(id);
            if (lessons == null)
            {
                return NotFound();
            }
            ViewData["CourseId"] = new SelectList(_context.Courses, "CourseId", "CourseId", lessons.CourseId);
            return View(lessons);
        }

        // POST: Lesson/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("LessonId,Title,Text,CourseId")] Lessons lessons)
        {
            if (id != lessons.LessonId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(lessons);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!LessonsExists(lessons.LessonId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["CourseId"] = new SelectList(_context.Courses, "CourseId", "CourseId", lessons.CourseId);
            return View(lessons);
        }

        // GET: Lesson/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var lessons = await _context.Lessons
                .Include(l => l.Course)
                .FirstOrDefaultAsync(m => m.LessonId == id);
            if (lessons == null)
            {
                return NotFound();
            }

            return View(lessons);
        }

        // POST: Lesson/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var lessons = await _context.Lessons.FindAsync(id);
            if (lessons != null)
            {
                _context.Lessons.Remove(lessons);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool LessonsExists(int id)
        {
            return _context.Lessons.Any(e => e.LessonId == id);
        }
    }
}
