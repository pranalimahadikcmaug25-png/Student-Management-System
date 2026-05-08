using StudentManagementSystem.Models;

namespace StudentManagementSystem.Repositories.Interfaces
{
    public interface IUserRepository
    {
        Task<User?> GetByUsernameAsync(string username);
        Task<User> CreateAsync(User user);
        Task<bool> UsernameExistsAsync(string username);
    }
}
