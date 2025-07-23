using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace DATN.Models;

[Index("ProductName", Name = "UQ__Products__DD5A978ADD220D96", IsUnique = true)]
public partial class Product
{
    [Key]
    [Column("ProductID")]
    public int ProductId { get; set; }

    [StringLength(200)]
    public string? ProductName { get; set; }

    [StringLength(225)]
    public string? Description { get; set; }

    [Column(TypeName = "decimal(18, 2)")]
    public decimal? SalePrice { get; set; }

    [Column(TypeName = "decimal(18, 2)")]
    public decimal? OriginalPrice { get; set; }

   

    [StringLength(50)]
    public string? Size { get; set; }

    [StringLength(50)]
    public string? Color { get; set; }

    [StringLength(100)]
    public string? Material { get; set; }

    [Column("CategoryID")]
    public int? CategoryId { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? CreatedDate { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? UpdatedDate { get; set; }

    [StringLength(225)]
    public string? ThumbnailImage { get; set; }

    [StringLength(50)]
    public string? Status { get; set; }

    [NotMapped]
    public IFormFile? ImageFile { get; set; }  // <-- ảnh upload cho từng biến thể
    [ForeignKey("CategoryId")]
    [InverseProperty("Products")]
    public virtual Category? Category { get; set; }

    [InverseProperty("Product")]
    public virtual ICollection<ProductVariant> ProductVariants { get; set; } = new List<ProductVariant>();
}
