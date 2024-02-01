
using MassTransit;

using SearchService.Core.Commands;

namespace SearchService
{



    public class SearchServices(IRequestClient<UserSearchRequest> UserSearchRequestClient, IRequestClient<ProductSearchRequest> ProductSearchRequestClient, ILogger<SearchServices> logger)
    {
        private readonly IRequestClient<UserSearchRequest> _UserSearchRequestClient = UserSearchRequestClient;
        private readonly IRequestClient<ProductSearchRequest> _ProductSearchRequestClient = ProductSearchRequestClient;
        private readonly ILogger<SearchServices> _logger = logger;

        public async Task<List<object>> SearchProductAsync(string text)
        {
            var searchRequest = new ProductSearchRequest { Text = text };
            _logger.LogInformation("Отправка запроса на поиск продуктов: {Text}", searchRequest.Text);

            var response = await _ProductSearchRequestClient.GetResponse<ProductSearchResponse>(searchRequest);

            _logger.LogInformation("Получен ответ на запрос поиска продуктов");

            return response.Message.Data;
        }

        public async Task<List<object>> SearchUserAsync(string text)
        {
            var searchRequest = new UserSearchRequest { Text = text };
            _logger.LogInformation("Отправка запроса на поиск пользователей: {Text}", searchRequest.Text);

            var response = await _UserSearchRequestClient.GetResponse<UserSearchResponse>(searchRequest);

            _logger.LogInformation("Получен ответ на запрос поиска пользователей");

            return response.Message.Data;
        }
    }

}
