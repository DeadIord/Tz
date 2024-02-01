using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrderService.Core.Commands
{
    public class GetProductsResponse 
    {
        public List<ProductsInfo> Products { get; set; }
    }

    public class ProductsInfo
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public double Price { get; set; }
    }
}
