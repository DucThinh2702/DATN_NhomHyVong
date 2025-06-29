using System;
using System.Collections.Generic;

namespace DATN.Models;

public partial class Order
{
    public int OrderId { get; set; }

    public int? UserId { get; set; }

    public DateTime? OrderDate { get; set; }

    public int? Quantity { get; set; }

    public decimal? TotalAmount { get; set; }

    public int? PaymentMethodId { get; set; }

    public string? PaymentStatus { get; set; }

    public string? OrderStatus { get; set; }

    public string? RecipientName { get; set; }

    public string? RecipientPhone { get; set; }

    public string? DeliveryAddress { get; set; }

    public string? Note { get; set; }

    public int? DiscountCode { get; set; }

    public decimal? ShippingFee { get; set; }

    public virtual Promotion? DiscountCodeNavigation { get; set; }

    public virtual ICollection<OrderDetail> OrderDetails { get; set; } = new List<OrderDetail>();

    public virtual PaymentMethod? PaymentMethod { get; set; }

    public virtual ICollection<Payment> Payments { get; set; } = new List<Payment>();

    public virtual User? User { get; set; }
}
