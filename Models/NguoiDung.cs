using System.ComponentModel.DataAnnotations;

namespace DATN.Models
{
    public class NguoiDung
    {
        [Key]

        public string MaND { get; set; }
        public string TenND { get; set; }
        public string DiaChi { get; set; }
        public string DienThoai { get; set; }
        public string Email { get; set; }
        public DateTime? NgaySinh { get; set; }
        public string GioiTinh { get; set; }
        public string MatKhau { get; set; }
        public string TrangThai { get; set; }
        public DateTime NgayTao { get; set; }

        public ICollection<TaiKhoan> TaiKhoans { get; set; }
        public ICollection<DonHang> DonHangs { get; set; }
        public ICollection<GioHang> GioHangs { get; set; }
    }

}
