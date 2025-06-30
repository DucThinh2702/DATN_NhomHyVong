using System.ComponentModel.DataAnnotations;

namespace DATN.Models
{
    public class GioHang
    {
        [Key]

        public string MaGH { get; set; }
        public string MaND { get; set; }
        public DateTime NgayTao { get; set; }

        public NguoiDung NguoiDung { get; set; }
        public ICollection<ChiTietGioHang> ChiTietGioHangs { get; set; }
    }

}
