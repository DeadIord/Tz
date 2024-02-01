using Microsoft.EntityFrameworkCore;
using ProductService.Data;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ProductService.Models
{
    public class ProductServices(IProductRepository productRepository)
    {
        private readonly IProductRepository _productRepository = productRepository;

        public async Task<List<Product>> GetAllProductsAsync()
        {
            return await _productRepository.GetAllProductsAsync();
        }

        public async Task<Product> CreateProduct(Product product)
        {
            return await _productRepository.CreateProduct(product);
        }

        public async Task<Product> GetProductById(int productId)
        {
            return await _productRepository.GetProductById(productId);
        }

        public async Task<Product> UpdateProduct(int productId, Product updatedProduct)
        {
            return await _productRepository.UpdateProduct(productId, updatedProduct);
        }

        public async Task<bool> DeleteProduct(int productId)
        {
            return await _productRepository.DeleteProduct(productId);
        }
    }

}
