using MassTransit;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SearchService.Core.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UserService.Data;

namespace UserService.Rabbit
{

    public class SearchRequestHandler(AddDbContext dbContext, ILogger<SearchRequestHandler> logger) : IConsumer<UserSearchRequest>
    {
        private readonly AddDbContext _dbContext = dbContext;
        private readonly ILogger<SearchRequestHandler> _logger = logger;

        public async Task Consume(ConsumeContext<UserSearchRequest> context)
        {
            var searchRequest = context.Message;

            _logger.LogInformation("Получен запрос на поиск: {Text}", searchRequest.Text);

            var searchResult = await PerformSearch(searchRequest.Text);

            _logger.LogInformation("Поиск завершен. Найдено {Count} результатов", searchResult.Count);

            var searchResponse = new UserSearchResponse { Data = searchResult };

            await context.RespondAsync(searchResponse);
        }

        private async Task<List<object>> PerformSearch(string searchText)
        {
            var users = await _dbContext.Users
                .Where(p => EF.Functions.Like(p.Username, $"%{searchText}%"))
                .Select(p => new { p.Username })
                .ToListAsync();

            var results = users.Cast<object>().ToList();
            return results;
        }

    }
}