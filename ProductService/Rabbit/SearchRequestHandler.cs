using MassTransit;
using Microsoft.EntityFrameworkCore;
using ProductService.Data;
using ProductService.Models;
using SearchService.Core.Commands;

namespace ProductService.Rabbit
{



    public class SearchRequestHandler(AddProductDbContext dbContext, ILogger<SearchRequestHandler> logger) : IConsumer<ProductSearchRequest>
    {
        private readonly AddProductDbContext _dbContext = dbContext;
        private readonly ILogger<SearchRequestHandler> _logger = logger;

        public async Task Consume(ConsumeContext<ProductSearchRequest> context)
        {
            var searchRequest = context.Message;

            _logger.LogInformation("Получен запрос на поиск: {Text}", searchRequest.Text);

            var searchResult = await PerformSearch(searchRequest.Text);

            _logger.LogInformation("Поиск завершен. Найдено {Count} результатов", searchResult.Count);

            var searchResponse = new ProductSearchResponse { Data = searchResult };

            await context.RespondAsync(searchResponse);
        }

        private async Task<List<object>> PerformSearch(string searchText)
        {
            var users = await _dbContext.Product
                .Where(p => EF.Functions.Like(p.Name, $"%{searchText}%"))
                .Select(p => new { p.Name })
                .ToListAsync();

            var results = users.Cast<object>().ToList();
            return results;
        }

    }
}