using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace DATN.Models;

[Index("CartId", "VariantId", Name = "UQ_Cart_Variant", IsUnique = true)]
public partial class CartDetail
{
    [Key]
    [Column("CartDetailID")]
    public int CartDetailId { get; set; }

    [Column("CartID")]
    public int? CartId { get; set; }

    [Column("VariantID")]
    public int? VariantId { get; set; }

    public int? Quantity { get; set; }

    [ForeignKey("CartId")]
    [InverseProperty("CartDetails")]
    public virtual Cart? Cart { get; set; }

    [ForeignKey("VariantId")]
    [InverseProperty("CartDetails")]
    public virtual ProductVariant? Variant { get; set; }
}
