using System.ComponentModel.DataAnnotations;

namespace DATN.Models
{
    public class OrderDetail
    {
        [Key]
        public string MaCTDH { get; set; }

        [Required]
        public string MaDH { get; set; }

        [Required]
        public string MaSP { get; set; }

        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "Số lượng phải lớn hơn 0")]
        public int SoLuong { get; set; }

        [Required]
        [DataType(DataType.Currency)]
        public decimal DonGia { get; set; }

        [Required]
        [DataType(DataType.Currency)]
        public decimal ThanhTien { get; set; }
    }
}
