using System.ComponentModel.DataAnnotations;

namespace DATN.Models
{
    public class ChiTietDonHang
    {
        [Key]
        public string MaCTDH { get; set; }
        public string MaDH { get; set; }
        public string MaSP { get; set; }
        public int SoLuong { get; set; }
        public decimal DonGia { get; set; }
        public decimal KhuyenMai { get; set; }

        public DonHang DonHang { get; set; }
        public SanPham SanPham { get; set; }
    }

}
