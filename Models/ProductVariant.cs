using System;
using System.ComponentModel.DataAnnotations;

namespace DATN.Models
{
    public class ProductVariant
    {
        [Key]
        public int VariantID { get; set; }

        public int ProductID { get; set; }
        public int ColorID { get; set; }
        public int SizeID { get; set; }

        public string SKU { get; set; }
        public int Stock { get; set; }

        public decimal? SalePrice { get; set; }
        public decimal? OriginalPrice { get; set; }

        public string? ThumbnailImage { get; set; }
        public string? Status { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime? UpdatedDate { get; set; }

        // Navigation
        public Product? Product { get; set; }
        public Color? Color { get; set; }
        public Size? Size { get; set; }
    }
}
