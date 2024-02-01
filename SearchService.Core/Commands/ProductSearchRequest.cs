using System.Collections.Generic;

namespace SearchService.Core.Commands
{
    public class ProductSearchRequest
    {
        public string Text { get; set; }
    
    }

    public class ProductSearchResponse
    {
        public List<object> Data { get; set; }
    }


}