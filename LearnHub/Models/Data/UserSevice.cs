using LearnHub.Interfaces;
using LearnHub.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;
using System.Text;
namespace LearnHub.Models.Data
{
    public class UserService : IUserService
    {
        private readonly LearnHubContext _context;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public UserService(LearnHubContext context, IHttpContextAccessor httpContextAccessor)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _httpContextAccessor = httpContextAccessor ?? throw new ArgumentNullException(nameof(httpContextAccessor));
        }

        public async Task<bool> IsUserAuthenticatedAsync()
        {
            var currentUser = _httpContextAccessor.HttpContext.Session.GetString("CurrentUser");
            return !string.IsNullOrEmpty(currentUser);
        }

        public async Task CreateUserAsync(Users user)
        {
            try
            {
                user.RegistrationDate = DateTime.UtcNow;
                user.Avatar = "https://yt3.googleusercontent.com/ytc/AIf8zZS5-w-s2K8_JFeHXG9Tb0ehxfyYGSgR4y9kvWZSgQ=s900-c-k-c0x00ffffff-no-rj";
                user.Role = "User";
                _context.Users.Add(user);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                throw new Exception($"Ошибка при регистрации: {ex.Message}");
            }
        }

        public async Task<Users> GetUserByIdAsync(int userId)
        {
            return await _context.Users.FindAsync(userId);
        }

        public async Task<Users> GetUserByUsernameAsync(string username)
        {
            return await _context.Users.FirstOrDefaultAsync(u => u.Username == username);
        }

        public async Task<Users> GetUserByEmailAsync(string email)
        {
            return await _context.Users.FirstOrDefaultAsync(m => m.Email == email);
        }

        public async Task<Courses> GetLastStartedCourseAsync(int userId)
        {
            var lastEnrollment = await _context.Enrollments
                .Where(e => e.UserId == userId)
                .OrderByDescending(e => e.EnrollmentDate)
                .FirstOrDefaultAsync();

            if (lastEnrollment != null)
            {
                return await _context.Courses.FindAsync(lastEnrollment.CourseId);
            }

            return null;
        }

        public async Task UpdateUserAsync(Users user)
        {
            try
            {
                _context.Entry(user).State = EntityState.Modified;
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                throw new Exception($"Ошибка при обновлении пользователя: {ex.Message}");
            }
        }
    }
}