using LearnHub.Models;

namespace LearnHub.Interfaces
{
    public interface IUserService
    {
        Task<bool> IsUserAuthenticatedAsync();
        Task CreateUserAsync(Users user);
        Task<Users> GetUserByIdAsync(int userId);
        Task<Users> GetUserByUsernameAsync(string username);
        Task<Courses> GetLastStartedCourseAsync(int userId);
        Task<Users> GetUserByEmailAsync(string email);
        Task UpdateUserAsync(Users user);
    }
}