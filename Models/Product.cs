using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace DATN.Models;

public partial class Product
{
    //public int ProductId { get; set; }

    //public string? ProductName { get; set; }

    //public string? Description { get; set; }

    //public decimal? SalePrice { get; set; }

    //public decimal? OriginalPrice { get; set; }

    //public int? Stock { get; set; }

    //public string? Size { get; set; }

    //public string? Color { get; set; }

    //public string? Material { get; set; }

    //public int? CategoryId { get; set; }

    //public DateTime? CreatedDate { get; set; }

    //public DateTime? UpdatedDate { get; set; }

    //public string? ThumbnailImage { get; set; }

    //public string? Status { get; set; }

    //public virtual ICollection<CartDetail> CartDetails { get; set; } = new List<CartDetail>();

    //public virtual Category? Category { get; set; }

    //public virtual ICollection<OrderDetail> OrderDetails { get; set; } = new List<OrderDetail>();

    public int ProductId { get; set; }

    [Required(ErrorMessage = "Tên sản phẩm không được để trống")]
    public string ProductName { get; set; }

    public decimal Price { get; set; }

    // Các thuộc tính khác nếu có
}
