using System;
using System.Collections.Generic;

namespace DATN.Models;

public partial class Promotion
{
    public int PromoCode { get; set; }

    public string? PromoName { get; set; }

    public string? PromoType { get; set; }

    public decimal? DiscountValue { get; set; }

    public decimal? MinOrderAmount { get; set; }

    public DateTime? StartDate { get; set; }

    public DateTime? EndDate { get; set; }

    public int? Quantity { get; set; }

    public int? UsedQuantity { get; set; }

    public string? Status { get; set; }

    public virtual ICollection<Order> Orders { get; set; } = new List<Order>();
}
