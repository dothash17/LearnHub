using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using LearnHub.Models;
using LearnHub.Models.Data;
using LearnHub.Interfaces;
using System.Globalization;

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

        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Courses course)
        {
            var currentUser = HttpContext.Session.GetString("CurrentUser");
            var user = await _userService.GetUserByUsernameAsync(currentUser);
            course.UserId = user.UserId;

            _context.Add(course);
            await _context.SaveChangesAsync();
            return RedirectToAction("Details", new {id = course.CourseId});
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
            ViewBag.CourseOnDraft = course.Status == "Draft";
            return View(course);
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var course = await _context.Courses.FirstOrDefaultAsync(c => c.CourseId == id);           
            return View(course);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(Courses course)
        {
            _context.Courses.Update(course);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Teaching));
        }

        [HttpGet]
        public async Task<IActionResult> Promo(int id)
        {          
            var currentUser = HttpContext.Session.GetString("CurrentUser");
            var user = await _userService.GetUserByUsernameAsync(currentUser);

            var course = await _context.Courses
                .Include(u => u.User)
                .Include(l => l.Lessons)
                .Include(e => e.Enrollments)
                .Include(g => g.Grades.OrderByDescending(s => s.Date))
                    .ThenInclude(grade => grade.User)
                .FirstOrDefaultAsync(c => c.CourseId == id);

            var isEnrolled = course.Enrollments.Any(e => e.UserId == user?.UserId);
            ViewBag.IsEnrolled = isEnrolled;
            ViewBag.User = user?.UserId;

            if (course.Grades.Any())
            {
                ViewBag.AverageGrade = Math.Round(course.Grades.Average(g => g.Grade), 1);
                var countGrade = course.Grades.Count();
                var lastDigit = countGrade % 10;

                if (countGrade == 0)
                {
                    ViewBag.CountGrades = "отзывов";
                }
                else if (countGrade == 1)
                {
                    ViewBag.CountGrades = "отзыв";
                }
                else if (lastDigit >= 2 && lastDigit <= 4)
                {
                    ViewBag.CountGrades = "отзыва";
                }
                else
                {
                    ViewBag.CountGrades = "отзывов";
                }

                ViewBag.CountGrade = countGrade;

                var userReview = course?.Grades?.FirstOrDefault(g => g.UserId == user?.UserId);
                ViewBag.UserReview = userReview;
            }

            return View(course);
        }

        [HttpPost]
        public async Task<IActionResult> Promo(Courses courses)
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

        [HttpPost]
        public async Task<IActionResult> LeaveReview(int courseId, int grade, string comment, int? reviewId)
        {
            var currentUser = HttpContext.Session.GetString("CurrentUser");
            var user = await _userService.GetUserByUsernameAsync(currentUser);

            Grades review;

            if (reviewId.HasValue)
            {
                review = await _context.Grades.FirstOrDefaultAsync(g => g.GradeId == reviewId.Value);

                if (review != null)
                {
                    review.Grade = (byte)grade;
                    review.Comment = comment;
                    review.Date = DateTime.UtcNow;
                }
            }
            else
            {
                review = new Grades
                {
                    CourseId = courseId,
                    UserId = user.UserId,
                    Grade = (byte)grade,
                    Comment = comment,
                    Date = DateTime.UtcNow
                };
                _context.Grades.Add(review);
            }

            await _context.SaveChangesAsync();

            return RedirectToAction("Promo", new { id = courseId });
        }

        [HttpPost]
        public async Task<IActionResult> DeleteReview(int reviewId)
        {
            var review = await _context.Grades.FirstOrDefaultAsync(g => g.GradeId == reviewId);

            _context.Grades.Remove(review);
            await _context.SaveChangesAsync();

            return RedirectToAction("Promo", new { id = review.CourseId });
        }

        [HttpPost]
        public async Task<IActionResult> SendToModeration(int courseId)
        {
            var course = await _context.Courses.FindAsync(courseId);
            course.Status = "Moderation";
            await _context.SaveChangesAsync();
            return RedirectToAction("Details", new { id = courseId });
        }

        [HttpGet]
        public IActionResult Moderation()
        {
            var moderationCourses = _context.Courses.Where(c => c.Status == "Moderation").ToList();
            return View(moderationCourses);
        }

        [HttpPost]
        public async Task<IActionResult> PublishCourse(int courseId)
        {
            var course = await _context.Courses.FindAsync(courseId);
            course.Status = "Published";
            course.PublicationDate = DateTime.UtcNow;
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Moderation));
        }

        [HttpPost]
        public async Task<IActionResult> SendToDraft(int courseId)
        {
            var course = await _context.Courses.FindAsync(courseId);
            course.Status = "Draft";
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Moderation));
        }
    }
}
