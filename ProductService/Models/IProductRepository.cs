using ProductService.Data;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ProductService.Models
{
    public interface IProductRepository
    {
        Task<List<Product>> GetAllProductsAsync();
        Task<Product> GetProductById(int productId);
        Task<Product> CreateProduct(Product product);
        Task<Product> UpdateProduct(int productId, Product updatedProduct);
        Task<bool> DeleteProduct(int productId);
    }

}
