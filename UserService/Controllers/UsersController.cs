using Microsoft.AspNetCore.Mvc;
using System.Security.Cryptography;
using System.Text;
using UserService.Data;
using UserService.Models;

namespace UserService.Controllers
{
   
    [ApiController]
    [Route("api/users")]
    public class UsersController(UserServices userService) : ControllerBase
    {
        private readonly UserServices _userService = userService;

        /// <summary>
        /// Аутентификация пользователя.
        /// </summary>
        /// <param name="Username">Логин пользователя.</param>
        /// <param name="Password">Пароль пользователя.</param>
        /// <returns>Результат аутентификации.</returns>
        [HttpPost("authenticate")]
        public async Task<IActionResult> AuthenticateAsync(string Username, string Password)
        {
            var response = await _userService.AuthenticateAsync(Username, Password);
            if (response == null)
                return BadRequest(new { message = "Неверные учетные данные" });
            return Ok(response);
        }

        /// <summary>
        /// Получает список всех пользователей.
        /// </summary>
        /// <returns>Список пользователей.</returns>
        [HttpGet]
        public async Task<IActionResult> GetAllUser()
        {
            var users = await _userService.GetAllUserAsync();
            if (users == null)
                return NotFound();
            return Ok(users);
        }

        /// <summary>
        /// Получает историю заказов для пользователя по его идентификатору.
        /// </summary>
        /// <param name="userId">Идентификатор пользователя.</param>
        /// <returns>История заказов пользователя.</returns>
        [HttpGet("{userId}/ordersHistory")]
        public async Task<IActionResult> GetOrderHistoryAsync(int userId)
        {
            var orderHistory = await _userService.GetOrderHistoryForUserAsync(userId);
            if (orderHistory.Count == 0)
                return BadRequest(new { message = "Данные отсутствуют" });
            return Ok(orderHistory);
        }

        /// <summary>
        /// Получает детали заказа для пользователя по идентификаторам заказа и пользователя.
        /// </summary>
        /// <param name="orderId">Идентификатор заказа.</param>
        /// <param name="userId">Идентификатор пользователя.</param>
        /// <returns>Детали заказа для пользователя.</returns>
        [HttpGet("{userId}/ordersDetails")]
        public async Task<IActionResult> GetOrderDetailsAsync(int orderId, int userId)
        {
            var orderDetails = await _userService.GetOrderDetailsAsync(orderId, userId);
            if (orderDetails == null)
                return BadRequest(new { message = "Данные отсутствуют" });
            return Ok(orderDetails);
        }

        /// <summary>
        /// Создает нового пользователя.
        /// </summary>
        /// <param name="username">Имя пользователя.</param>
        /// <param name="password">Пароль пользователя.</param>
        /// <returns>Созданный пользователь.</returns>
        [HttpPost("create")]
        public IActionResult Create(string username, string password)
        {
            if (_userService.UserExists(username))
            {
                return BadRequest(new { message = "Пользователь с таким именем уже существует" });
            }

            string passwordHash = HashPassword(password, out byte[] salt);
            var user = new User() { Username = username, PasswordHash = passwordHash, Salt = salt };
            var createdUser = _userService.Create(user);
            if (createdUser == null)
                return BadRequest(new { message = "Некорректные данные" });
            return CreatedAtAction(nameof(GetUser), new { userId = createdUser.Id }, createdUser);
        }

        /// <summary>
        /// Получает пользователя по его идентификатору.
        /// </summary>
        /// <param name="userId">Идентификатор пользователя.</param>
        /// <returns>Пользователь с указанным идентификатором.</returns>
        [HttpGet("{userId}")]
        public IActionResult GetUser(int userId)
        {
            var user = _userService.GetById(userId);
            if (user == null)
                return BadRequest(new { message = "Данный пользователь отсутствует" });
            return Ok(user);
        }

        /// <summary>
        /// Обновляет информацию о пользователе.
        /// </summary>
        /// <param name="userId">Идентификатор пользователя.</param>
        /// <param name="Usrname">Новое имя пользователя.</param>
        /// <param name="password">Новый пароль пользователя.</param>
        /// <returns>Обновленный пользователь.</returns>
        [HttpPut("{userId}")]
        public IActionResult UpdateUser(int userId, string Usrname, string password)
        {
            string passwordHash = HashPassword(password, out byte[] salt);
            var updatedUser = new User() { Username = Usrname, PasswordHash = passwordHash, Salt = salt };
            var user = _userService.Update(userId, updatedUser);
            if (user == null)
                return BadRequest(new { message = "Данный пользователь отсутствует" });
            return Ok(user);
        }

        /// <summary>
        /// Удаляет пользователя по его идентификатору.
        /// </summary>
        /// <param name="userId">Идентификатор пользователя для удаления.</param>
        /// <returns>Результат операции удаления.</returns>
        [HttpDelete("{userId}")]
        public IActionResult DeleteUser(int userId)
        {
            var user = _userService.Delete(userId);
            if (user == null)
                return BadRequest(new { message = "Данный пользователь отсутствует" });
            return NoContent();
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
