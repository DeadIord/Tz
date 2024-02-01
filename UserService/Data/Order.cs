using System;
using System.Collections.Generic;

namespace UserService.Data
{
    public class Order
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public double TotalCost { get; set; }
        public DateTime OrderDate { get; set; }
         public virtual ICollection<OrderItem> OrderItems { get; set; }

        public User User { get; set; }

    }



}
