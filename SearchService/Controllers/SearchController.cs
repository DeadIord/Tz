using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

namespace SearchService.Controllers
{
    public class SearchController(SearchServices searchService, ILogger<SearchController> logger) : ControllerBase
    {
        private readonly SearchServices _searchService = searchService;
        private readonly ILogger<SearchController> _logger = logger;

        /// <summary>
        /// Выполняет поиск продуктов.
        /// </summary>
        /// <param name="text">Текст запроса.</param>
        /// <returns>Результаты поиска продуктов.</returns>
        [HttpPost("SearchProduct")]
        public async Task<IActionResult> SearchProductAsync(string text)
        {
            _logger.LogInformation("Получен запрос на поиск продуктов");
            var searchResults = await _searchService.SearchProductAsync(text);

            if (searchResults.Count == 0)
            {
                _logger.LogInformation("Результаты поиска продуктов не найдены");
                return BadRequest(new { message = "Товары не найдены" });
            }

            _logger.LogInformation("Возвращены результаты поиска продуктов");
            return Ok(searchResults);
        }

        /// <summary>
        /// Выполняет поиск пользователей.
        /// </summary>
        /// <param name="text">Текст запроса.</param>
        /// <returns>Результаты поиска пользователей.</returns>
        [HttpPost("SearchUser")]
        public async Task<IActionResult> SearchUserAsync(string text)
        {
            _logger.LogInformation("Получен запрос на поиск пользователей");
            var searchResults = await _searchService.SearchUserAsync(text);

            if (searchResults.Count == 0)
            {
                _logger.LogInformation("Результаты поиска пользователей не найдены");
                return BadRequest(new { message = "Пользователи не найдены" });
            }

            _logger.LogInformation("Возвращены результаты поиска пользователей");
            return Ok(searchResults);
        }
    }
}