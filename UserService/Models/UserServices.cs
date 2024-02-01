using UserService.Data;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;
using System.Text;
using UserService.ViewModel;
using OrderService.Core.Commands;
using MassTransit;
using Microsoft.AspNetCore.Identity;
namespace UserService.Models
{
    public class UserServices(IUserRepository userRepository, AddDbContext dbContext, ILogger<UserServices> logger, IRequestClient<RequestForOrderHistory> getProductsRequestClient)
    {
        private readonly IUserRepository _userRepository = userRepository;
        private readonly AddDbContext _dbContext = dbContext;
        private readonly ILogger<UserServices> _logger = logger;
        private readonly IRequestClient<RequestForOrderHistory> _getProductsRequestClient = getProductsRequestClient;
        public async Task<User> AuthenticateAsync(string username, string password)
        {
            var user = await _userRepository.GetUserByUsernameAsync(username);
            if (user == null || !VerifyPasswordHash(password, user.PasswordHash, user.Salt))
                return null;
            user.Token = GenerateToken();
            await _dbContext.SaveChangesAsync();
            return user;
        }
        public async Task<List<User>> GetAllUserAsync()
        {
            return await _userRepository.GetAllUsersAsync();
        }
        public async Task<OrderDetailsVM> GetOrderDetailsAsync(int orderId, int userId)
        {
            _logger.LogInformation("Получение деталей заказа для пользователя {UserId}, заказ {OrderId}", userId, orderId);
            var order = await _dbContext.Orders
                .Include(o => o.OrderItems)
                .FirstOrDefaultAsync(o => o.Id == orderId && o.UserId == userId);
            if (order == null)
            {
                _logger.LogWarning("Заказ с Id {OrderId} для пользователя {UserId} не найден", orderId, userId);
                return null;
            }
            var productIds = order.OrderItems.Select(oi => oi.ProductId).ToList();
            _logger.LogInformation("Отправка запроса на получение информации о продуктах по Ids: {ProductIds}", string.Join(", ", productIds));
            var getProductsRequest = new RequestForOrderHistory
            {
                ProductIds = productIds
            };
            var getProductsResponse = await _getProductsRequestClient.GetResponse<GetProductsResponse>(getProductsRequest);
            var productsInfo = getProductsResponse.Message.Products;
            var orderDetails = new OrderDetailsVM
            {
                OrderId = order.Id,
                OrderDate = order.OrderDate,
                TotalCost = order.TotalCost,
                OrderItems = order.OrderItems.Select(oi => new OrderItemVM
                {
                    ProductName = productsInfo.FirstOrDefault(p => p.Id == oi.ProductId).Name,
                    Quantity = oi.Quantity,
                    Price = productsInfo.FirstOrDefault(p => p.Id == oi.ProductId).Price * oi.Quantity
                }).ToList()
            };
            _logger.LogInformation("Получены детали заказа для пользователя {UserId}, заказ {OrderId}", userId, orderId);
            return orderDetails;
        }
        public async Task<List<OrderHistoryItemVM>> GetOrderHistoryForUserAsync(int userId)
        {
            var orderHistory = await _dbContext.Orders
                .Where(o => o.UserId == userId)
                .Select(o => new OrderHistoryItemVM
                {
                    OrderId = o.Id,
                    OrderDate = o.OrderDate,
                    TotalCost = o.TotalCost,
                    TotalQuantity = o.OrderItems.Sum(oi => oi.Quantity)
                })
                .ToListAsync();
            return orderHistory;
        }
        public User Create(User user)
        {
            user.Token = GenerateToken();
            return _userRepository.CreateUser(user).Result;
        }
        public User GetById(int userId)
        {
            return _userRepository.GetUserById(userId).Result;
        }
        public User Update(int userId, User updatedUser)
        {
            return _userRepository.UpdateUser(userId, updatedUser).Result;
        }
        public bool Delete(int userId)
        {
            return _userRepository.DeleteUser(userId).Result;
        }
        public bool UserExists(string username)
        {
            return _userRepository.UserExists(username).Result;
        }
        private static bool VerifyPasswordHash(string password, string storedHash, byte[] salt)
        {
            using var hmac = new HMACSHA512();
            hmac.Key = salt;
            byte[] enteredPasswordBytes = Encoding.UTF8.GetBytes(password);
            byte[] enteredHashBytes = hmac.ComputeHash(enteredPasswordBytes);
            string enteredHashString = Convert.ToBase64String(enteredHashBytes);
            return enteredHashString == storedHash;
        }
        private static string GenerateToken()
        {
            using var random = RandomNumberGenerator.Create();
            byte[] tokenData = new byte[32];
            random.GetBytes(tokenData);
            return Convert.ToBase64String(tokenData);
        }
    }
}
