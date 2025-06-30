using System.ComponentModel.DataAnnotations;

namespace DATN.Models
{
    public class SanPham
    {
        [Key]

        public string MaSP { get; set; }
        public string TenSP { get; set; }
        public string MoTa { get; set; }
        public decimal GiaBan { get; set; }
        public decimal GiaGoc { get; set; }
        public int SoLuong { get; set; }
        public string HinhAnh { get; set; }
        public string MaDanhMuc { get; set; }
        public DateTime NgayTao { get; set; }
        public string TrangThai { get; set; }

        public DanhMuc DanhMuc { get; set; }
    }

}
