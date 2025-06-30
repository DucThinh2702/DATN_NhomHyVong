using System.ComponentModel.DataAnnotations;

namespace DATN.Models
{
    public class KhuyenMai
    {
        [Key]

        public string MaKM { get; set; }
        public string TenKM { get; set; }
        public decimal GiaTri { get; set; }
        public DateTime NgayBatDau { get; set; }
        public DateTime NgayKetThuc { get; set; }
    }

}
