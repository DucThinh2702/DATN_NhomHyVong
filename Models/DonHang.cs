using System.ComponentModel.DataAnnotations;

namespace DATN.Models
{
    public class DonHang
    {
        [Key]
        public string MaDH { get; set; }
        public string MaND { get; set; }
        public DateTime NgayDat { get; set; }
        public decimal TongTien { get; set; }
        public string TrangThai { get; set; }

        public NguoiDung NguoiDung { get; set; }
        public ICollection<ChiTietDonHang> ChiTietDonHangs { get; set; }
    
    }

}
