using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace DATN.Models;

[Index("PromoName", Name = "UQ__Promotio__BACB2629B204EA50", IsUnique = true)]
public partial class Promotion
{
    [Key]
    public int PromoCode { get; set; }

    [StringLength(100)]
    public string? PromoName { get; set; }

    [StringLength(50)]
    public string? PromoType { get; set; }

    [Column(TypeName = "decimal(18, 2)")]
    public decimal? DiscountValue { get; set; }

    [Column(TypeName = "decimal(18, 2)")]
    public decimal? MinOrderAmount { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? StartDate { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? EndDate { get; set; }

    public int? Quantity { get; set; }

    public int? UsedQuantity { get; set; }

    [StringLength(50)]
    public string? Status { get; set; }
}
