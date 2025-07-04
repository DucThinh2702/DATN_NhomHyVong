
using System.ComponentModel.DataAnnotations;

namespace DATN.Models
{
    public class Product
    {
        public int ProductId { get; set; }
        public string ProductName { get; set; }
        public string? Description { get; set; }
        public decimal? SalePrice { get; set; }
        public decimal? OriginalPrice { get; set; }
        public int? Stock { get; set; }
        public string? Size { get; set; } // chỉ dùng nếu không quản lý biến thể
        public string? Color { get; set; } // chỉ dùng nếu không quản lý biến thể
        public string? Material { get; set; }
        public int? CategoryId { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public string? ThumbnailImage { get; set; }
        public string? Status { get; set; }

        // Navigation
        public Category? Category { get; set; }
        public ICollection<ProductVariant>? ProductVariants { get; set; }
        public virtual ICollection<OrderDetail> OrderDetails { get; set; } = new List<OrderDetail>();
        public virtual ICollection<CartDetail> CartDetails { get; set; } = new List<CartDetail>();
    }

}

