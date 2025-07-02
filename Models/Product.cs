//using System.ComponentModel.DataAnnotations;

//namespace DATN.Models
//{
//    public class Product
//    {
//        public int ProductID { get; set; }

//        [Required]
//        [StringLength(200)]
//        public string ProductName { get; set; }

//        [StringLength(225)]
//        public string Description { get; set; }

//        [Required]
//        public decimal SalePrice { get; set; }

//        public decimal OriginalPrice { get; set; }

//        public int Stock { get; set; }

//        [StringLength(50)]
//        public string Size { get; set; }

//        [StringLength(50)]
//        public string Color { get; set; }

//        [StringLength(100)]
//        public string Material { get; set; }

//        public int CategoryID { get; set; }

//        public DateTime CreatedDate { get; set; }

//        public DateTime? UpdatedDate { get; set; }

//        [StringLength(225)]
//        public string ThumbnailImage { get; set; }

//        [StringLength(50)]
//        public string Status { get; set; }

//        //// Nếu muốn join Category
//        //public Category Category { get; set; }
//    }

//}
using System.ComponentModel.DataAnnotations;

namespace DATN.Models
{
    public class Product
    {
        public int ProductID { get; set; }
        public string ProductName { get; set; }
        public string? Description { get; set; }
        public decimal? SalePrice { get; set; }
        public decimal? OriginalPrice { get; set; }
        public int? Stock { get; set; }
        public string? Size { get; set; } // chỉ dùng nếu không quản lý biến thể
        public string? Color { get; set; } // chỉ dùng nếu không quản lý biến thể
        public string? Material { get; set; }
        public int? CategoryID { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public string? ThumbnailImage { get; set; }
        public string? Status { get; set; }

        // Navigation
        public Category? Category { get; set; }
        public ICollection<ProductVariant>? ProductVariants { get; set; }
    }

}

