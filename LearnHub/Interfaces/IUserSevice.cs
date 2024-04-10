using LearnHub.Models;
using System.Threading.Tasks;

namespace LearnHub.Interfaces
{
    public interface IUserService
    {
        Task<bool> IsUserAuthenticatedAsync();
        Task CreateUserAsync(Users user);
        Task<Users> GetUserByIdAsync(int userId);
        Task<Users> GetUserByUsernameAsync(string username);
        Task<Users> GetUserByEmailAsync(string email);
        Task UpdateUserAsync(Users user);
    }
}