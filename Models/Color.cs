using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace DATN.Models;

[Index("ColorName", Name = "UQ__Colors__C71A5A7B138F9717", IsUnique = true)]
public partial class Color
{
    [Key]
    [Column("ColorID")]
    public int ColorId { get; set; }

    [StringLength(50)]
    public string? ColorName { get; set; }

    [InverseProperty("Color")]
    public virtual ICollection<ProductVariant> ProductVariants { get; set; } = new List<ProductVariant>();
}
