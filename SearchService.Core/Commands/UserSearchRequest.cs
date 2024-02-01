using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SearchService.Core.Commands
{
   public  class UserSearchRequest
    {
        public string Text { get; set; }

    }

    public class UserSearchResponse
    {
        public List<object> Data { get; set; }
    }
}
