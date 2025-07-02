using System;
using System.ComponentModel.DataAnnotations;

namespace DATN.Models
{
    public class Order
    {
        [Key]
        public int OrderID { get; set; }

        [Required]
        public int UserID { get; set; }

        public DateTime OrderDate { get; set; } = DateTime.Now;

        [Required]
        public int Quantity { get; set; }

        [Required]
        public decimal TotalAmount { get; set; }

        [Required]
        public int PaymentMethodID { get; set; }

        public string PaymentStatus { get; set; }

        public string OrderStatus { get; set; }

        public string RecipientName { get; set; }

        public string RecipientPhone { get; set; }

        public string DeliveryAddress { get; set; }

        public string Note { get; set; }

        public int? DiscountCode { get; set; }

        public decimal ShippingFee { get; set; } = 0;
    }
}
