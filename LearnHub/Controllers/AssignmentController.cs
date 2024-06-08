using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using LearnHub.Models;
using LearnHub.Models.Data;

namespace LearnHub.Controllers
{
    public class AssignmentController : Controller
    {
        private readonly LearnHubContext _context;

        public AssignmentController(LearnHubContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var learnHubContext = _context.Assignments.Include(a => a.Lesson);
            return View(await learnHubContext.ToListAsync());
        }

        public IActionResult Create(int lessonId)
        {
            ViewBag.LessonId = lessonId;
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(Assignments assignments)
        {
            _context.Add(assignments);
            await _context.SaveChangesAsync();
            return RedirectToAction("Details", "Lesson", new { id = assignments.LessonId });
        }

        public async Task<IActionResult> Edit(int? id)
        {
            var assignments = await _context.Assignments.FindAsync(id);
            return View(assignments);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(Assignments assignments, int id)
        {
            _context.Update(assignments);
            await _context.SaveChangesAsync();   
            return RedirectToAction("Details", "Lesson", new { id = assignments.LessonId });
        }

        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            var assignment = await _context.Assignments.FindAsync(id);
            _context.Assignments.Remove(assignment);
            await _context.SaveChangesAsync();
            return RedirectToAction("Details", "Lesson", new { id = assignment.LessonId });
        }
    }
}
