using System.ComponentModel.DataAnnotations;

namespace DATN.Models
{
    public class ChiTietGioHang
    {
        [Key]
        public string MaCTGH { get; set; }
        public string MaGH { get; set; }
        public string MaSP { get; set; }
        public int SoLuong { get; set; }
        public decimal DonGia { get; set; }

        public GioHang GioHang { get; set; }
        public SanPham SanPham { get; set; }
    }

}
