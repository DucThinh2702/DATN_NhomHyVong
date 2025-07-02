using System.Collections.Generic;

namespace DATN.Models
{
    public class OrderCreateViewModel
    {
        public Order Order { get; set; }
        public List<OrderDetail> OrderDetails { get; set; }
    }
}
