using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace DATN.Models;

[Index("CategoryName", Name = "UQ__Categori__8517B2E0F9C2DE4E", IsUnique = true)]
public partial class Category
{
    [Key]
    [Column("CategoryID")]
    public int CategoryId { get; set; }

    [StringLength(100)]
    public string? CategoryName { get; set; }

    [StringLength(225)]
    public string? CategoryDescription { get; set; }

    [StringLength(225)]
    public string? CategoryImage { get; set; }

    [InverseProperty("Category")]
    public virtual ICollection<Product> Products { get; set; } = new List<Product>();
}
