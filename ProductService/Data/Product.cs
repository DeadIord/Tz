
using System.Collections.Generic;
using System.Text.Json.Serialization;


namespace ProductService.Data
{
    public class Product
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public double Price { get; set; }

    }

}
