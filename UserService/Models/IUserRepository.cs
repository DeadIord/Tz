using UserService.Data;

namespace UserService.Models
{
    public interface IUserRepository
    {
        Task<List<User>> GetAllUsersAsync();
        Task<User> GetUserByUsernameAsync(string username);
        Task<User> GetUserById(int userId);
        Task<User> CreateUser(User user);
        Task<User> UpdateUser(int userId, User updatedUser);
        Task<bool> DeleteUser(int userId);
        Task<bool> UserExists(string username);

    }
}
