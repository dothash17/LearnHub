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
        Task UpdateUserAsync(Users user);
        Task DeleteUserAsync(int userId);
    }
}