using System.ComponentModel.DataAnnotations;

namespace DATN.Models
{
    public class TaiKhoan
    {
        [Key]

        public string MaTK { get; set; }
        public string TenTK { get; set; }
        public string MatKhau { get; set; }
        public string MaND { get; set; }
        public string MaVT { get; set; }
        public DateTime NgayTao { get; set; }
        public string TrangThai { get; set; }

        public NguoiDung NguoiDung { get; set; }
        public VaiTro VaiTro { get; set; }
    }

}
