﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrderService.Core.Commands
{
    public class RequestForOrderHistory
    {
        public List<int> ProductIds { get; set; }
    }
  

}
