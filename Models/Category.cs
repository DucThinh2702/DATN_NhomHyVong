using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace DATN.Models
{
    public class Category
    {
        [Key] // Khóa chính
        public int CategoryId { get; set; }

        [Required(ErrorMessage = "Tên danh mục không được để trống")]
        [StringLength(100, ErrorMessage = "Tên danh mục tối đa 100 ký tự")]
        [Display(Name = "Tên danh mục")]
        public string CategoryName { get; set; }

        [Display(Name = "Ảnh danh mục")]
        public string? CategoryImage { get; set; }

        // Navigation property đến Product
        public virtual ICollection<Product>? Products { get; set; }

    }
}
