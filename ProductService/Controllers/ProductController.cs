using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using ProductService.Models;
using System.Threading.Tasks;
using ProductService.Data;
namespace ProductService.Controllers
{
    [ApiController]
    [Route("api/products")]
    public class ProductController(ProductServices ProducService) : ControllerBase
    {
        private readonly ProductServices _ProducService = ProducService;
        /// <summary>
        /// Получает список всех продуктов.
        /// </summary>
        /// <returns>Список продуктов.</returns>
        [HttpGet]
        public async Task<IActionResult> GetAllProductsAsync()
        {
            var products = await _ProducService.GetAllProductsAsync();
            if (products == null)
                return NotFound();
            return Ok(products);
        }

        /// <summary>
        /// Создает новый продукт.
        /// </summary>
        /// <param name="name">Название продукта.</param>
        /// <param name="price">Цена продукта.</param>
        /// <returns>Созданный продукт.</returns>
        [HttpPost("CreateProduct")]
        public async Task<IActionResult> CreateProduct(string name, double price)
        {
            var product = new Product() { Name = name, Price = price };
            var createdProduct = await _ProducService.CreateProduct(product);
            return CreatedAtAction(nameof(GetProductById), new { productId = createdProduct.Id }, createdProduct);
        }

        /// <summary>
        /// Получает продукт по его идентификатору.
        /// </summary>
        /// <param name="productId">Идентификатор продукта.</param>
        /// <returns>Продукт с указанным идентификатором.</returns>
        [HttpGet("GetProduct/{productId}")]
        public async Task<IActionResult> GetProductById(int productId)
        {
            var product = await _ProducService.GetProductById(productId);
            if (product == null)
                return BadRequest(new { message = "Данные отсутствуют" });
            return Ok(product);
        }

        /// <summary>
        /// Обновляет информацию о продукте.
        /// </summary>
        /// <param name="productId">Идентификатор продукта.</param>
        /// <param name="updatedProduct">Обновленная информация о продукте.</param>
        /// <returns>Обновленный продукт.</returns>
        [HttpPut("UpdateProduct/{productId}")]
        public async Task<IActionResult> UpdateProduct(int productId, [FromBody] Product updatedProduct)
        {
            if (updatedProduct == null)
            {
                return BadRequest(new { message = "Некорректные данные" });
            }

            var product = await _ProducService.UpdateProduct(productId, updatedProduct);
            if (product == null)
            {
                return BadRequest(new { message = "Данный продукт отсутствует" });
            }

            return Ok(product);
        }

        /// <summary>
        /// Удаляет продукт по его идентификатору.
        /// </summary>
        /// <param name="productId">Идентификатор продукта для удаления.</param>
        /// <returns>Результат операции удаления.</returns>
        [HttpDelete("DeleteProduct/{productId}")]
        public async Task<IActionResult> DeleteProduct(int productId)
        {
            var result = await _ProducService.DeleteProduct(productId);
            if (!result)
                return BadRequest(new { message = "Данный продукт отсутствует" });

            return NoContent();
        }
    }
}