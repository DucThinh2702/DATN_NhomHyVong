using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace DATN.Models;

public partial class Order
{
    [Key]
    [Column("OrderID")]
    public int OrderId { get; set; }

    [Column("UserID")]
    public int? UserId { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? OrderDate { get; set; }

    public int? Quantity { get; set; }

    [Column(TypeName = "decimal(18, 2)")]
    public decimal? TotalAmount { get; set; }

    [Column("PaymentMethodID")]
    public int? PaymentMethodId { get; set; }

    [StringLength(50)]
    public string? PaymentStatus { get; set; }

    [StringLength(100)]
    public string? OrderStatus { get; set; }

    [StringLength(100)]
    public string? RecipientName { get; set; }

    [StringLength(11)]
    public string? RecipientPhone { get; set; }

    [StringLength(225)]
    public string? DeliveryAddress { get; set; }

    [StringLength(255)]
    public string? Note { get; set; }

    public int? DiscountCode { get; set; }

    [Column(TypeName = "decimal(18, 2)")]
    public decimal? ShippingFee { get; set; }

    [InverseProperty("Order")]
    public virtual ICollection<OrderDetail> OrderDetails { get; set; } = new List<OrderDetail>();

    [ForeignKey("PaymentMethodId")]
    [InverseProperty("Orders")]
    public virtual PaymentMethod? PaymentMethod { get; set; }

    [InverseProperty("Order")]
    public virtual ICollection<Payment> Payments { get; set; } = new List<Payment>();

    [ForeignKey("UserId")]
    [InverseProperty("Orders")]
    public virtual User? User { get; set; }
}
