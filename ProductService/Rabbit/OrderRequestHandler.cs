using MassTransit;
using Microsoft.EntityFrameworkCore;
using OrderService.Core.Commands;
using ProductService.Data;

namespace ProductService.Rabbit
{

    public class OrderRequestHandler(ILogger<OrderRequestHandler> logger, AddProductDbContext dbContext) : IConsumer<RequestForOrderHistory>
    {
        private readonly ILogger<OrderRequestHandler> _logger = logger;
        private readonly AddProductDbContext _dbContext = dbContext;

        public async Task Consume(ConsumeContext<RequestForOrderHistory> context)
        {
            var request = context.Message;

            _logger.LogInformation("Получен запрос на получение информации о продуктах по Id");

            var products = await _dbContext.Product
                .Where(p => request.ProductIds.Contains(p.Id))
                .Select(p => new ProductsInfo
                {
                    Id = p.Id,
                    Name = p.Name,
                    Price = p.Price
                })
                .ToListAsync();

            var response = new GetProductsResponse
            {
                Products = products
            };

            _logger.LogInformation("Отправка ответа с информацией о продуктах");

            await context.RespondAsync(response);
        }
    }
}

