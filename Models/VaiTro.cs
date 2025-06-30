using System.ComponentModel.DataAnnotations;

namespace DATN.Models
{
    public class VaiTro
    {
        [Key]

        public string MaVT { get; set; }
        public string TenVT { get; set; }
        public string MoTa { get; set; }

        public ICollection<TaiKhoan> TaiKhoans { get; set; }
    }

}
