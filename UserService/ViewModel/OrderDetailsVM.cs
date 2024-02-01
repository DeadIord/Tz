using System.Collections.Generic;
using System;

namespace UserService.ViewModel
{  
        public class OrderDetailsVM
        {
            public int OrderId { get; set; }
            public DateTime OrderDate { get; set; }
            public double TotalCost { get; set; }
            public List<OrderItemVM> OrderItems { get; set; }
        }
}
