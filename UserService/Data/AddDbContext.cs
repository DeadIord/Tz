using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;
using System.Text;
namespace UserService.Data
{
    public class AddDbContext(DbContextOptions<AddDbContext> options) : DbContext(options)
    {
        public DbSet<User> Users { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderItem> OrderItem { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            SeedData(modelBuilder);
        }
        private static void SeedData(ModelBuilder modelBuilder)
        {
            var users = new List<User>
            {
                 new () { Id = 1, Username = "Пользователь1", PasswordHash = HashPassword("test1", out var salt1), Salt = salt1, Token = "someTokenValue1" },
                 new () { Id = 2, Username = "Пользователь2", PasswordHash = HashPassword("test2", out var salt2), Salt = salt2, Token = "someTokenValue2" },
                 new () { Id = 3, Username = "zz", PasswordHash = HashPassword("test3", out var salt3), Salt = salt3, Token = "someTokenValue3" }
            };
                        modelBuilder.Entity<User>().HasData(users);
            var orders = new List<Order>
            {
                 new () { Id = 1, UserId = 1, OrderDate = DateTime.Now.AddDays(-2), TotalCost = 134000 },
                 new () { Id = 2, UserId = 1, OrderDate = DateTime.Now.AddDays(-1), TotalCost = 130000 },
                 new () { Id = 3, UserId = 2, OrderDate = DateTime.Now, TotalCost = 105000 }
            };
            modelBuilder.Entity<Order>().HasData(orders);
            var orderItems = new List<OrderItem>
            {
                new () { Id = 1, ProductId = 1, Quantity = 2, OrderId = 1 },
                new () { Id = 2, ProductId = 2, Quantity = 1, OrderId = 1 },
                new () { Id = 3, ProductId = 4, Quantity = 1, OrderId = 2 },
                new () { Id = 4, ProductId = 5, Quantity = 1, OrderId = 2 },
                new () { Id = 5, ProductId = 3, Quantity = 3, OrderId = 3 },
                new () { Id = 6, ProductId = 6, Quantity = 1, OrderId = 3 },
                new () { Id = 7, ProductId = 7, Quantity = 2, OrderId = 3 }
            };
            modelBuilder.Entity<OrderItem>().HasData(orderItems);
        }
        private static string HashPassword(string password, out byte[] salt)
        {
            using var hmac = new HMACSHA512();
            salt = new byte[32];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(salt);
            }
            hmac.Key = salt;
            byte[] passwordBytes = Encoding.UTF8.GetBytes(password);
            byte[] hashedBytes = hmac.ComputeHash(passwordBytes);
            return Convert.ToBase64String(hashedBytes);
        }
    }
}
