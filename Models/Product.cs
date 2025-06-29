
using System.ComponentModel.DataAnnotations;

namespace DATN.Models
{
    public class Product
    {
        [Key]
        public string MaSP { get; set; }

        [Required]
        public string TenSP { get; set; }

        [Required]
        public decimal GiaBan { get; set; }

        public string HinhAnhDaiDien { get; set; }
    }
}