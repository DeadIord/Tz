using Microsoft.EntityFrameworkCore;
using UserService.Data;

namespace UserService.Models
{
    public class UserRepository(AddDbContext dbContext) : IUserRepository
    {
        private readonly AddDbContext _dbContext = dbContext;

        public async Task<List<User>> GetAllUsersAsync()
        {
            return await _dbContext.Users.ToListAsync();
        }
        public async Task<User> GetUserByUsernameAsync(string username)
        {
            return await _dbContext.Users.SingleOrDefaultAsync(x => x.Username == username);
        }
        public async Task<User> GetUserById(int userId)
        {
            return await _dbContext.Users.FindAsync(userId);
        }

        public async Task<User> CreateUser(User user)
        {
            _dbContext.Users.Add(user);
            await _dbContext.SaveChangesAsync();
            return user;
        }

        public async Task<User> UpdateUser(int userId, User updatedUser)
        {
            var user = await _dbContext.Users.FindAsync(userId);

            if (user == null)
                return null;

            user.Username = updatedUser.Username;
            user.PasswordHash = updatedUser.PasswordHash;
            await _dbContext.SaveChangesAsync();
            return user;
        }

        public async Task<bool> DeleteUser(int userId)
        {
            var user = await _dbContext.Users.FindAsync(userId);

            if (user == null)
                return false;

            _dbContext.Users.Remove(user);
            await _dbContext.SaveChangesAsync();
            return true;
        }

        public async Task<bool> UserExists(string username)
        {
            return await _dbContext.Users.AnyAsync(x => x.Username == username);
        }
    }
}

