using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace DATN.Models;

[Index("SizeName", Name = "UQ__Sizes__619EFC3E98101D64", IsUnique = true)]
public partial class Size
{
    [Key]
    [Column("SizeID")]
    public int SizeId { get; set; }

    [StringLength(50)]
    public string? SizeName { get; set; }

    [InverseProperty("Size")]
    public virtual ICollection<ProductVariant> ProductVariants { get; set; } = new List<ProductVariant>();
}
