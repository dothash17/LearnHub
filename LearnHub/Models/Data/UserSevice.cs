using LearnHub.Interfaces;
using LearnHub.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
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