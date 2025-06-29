using System;
using System.ComponentModel.DataAnnotations;

namespace DATN.Models
{
    public class Order
    {
        [Key]
        public string MaDH { get; set; }

        [Required]
        public string MaKH { get; set; }

        [Display(Name = "Ngày đặt hàng")]
        public DateTime NgayDatHang { get; set; } = DateTime.Now;

        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "Số lượng phải lớn hơn 0")]
        public int SoLuong { get; set; }

        [Required]
        [Display(Name = "Tổng tiền")]
        [DataType(DataType.Currency)]
        public decimal TongTien { get; set; }

        [Required]
        [Display(Name = "Phương thức thanh toán")]
        public string MaPTTT { get; set; }

        [Display(Name = "Trạng thái thanh toán")]
        public string TrangThaiThanhToan { get; set; }

        [Display(Name = "Trạng thái đơn hàng")]
        public string TrangThaiDonHang { get; set; }

        [Display(Name = "Tên người nhận")]
        public string TenNguoiNhan { get; set; }

        [Display(Name = "SĐT người nhận")]
        public string SoDienThoaiNguoiNhan { get; set; }

        [Display(Name = "Địa chỉ giao hàng")]
        public string DiaChiGiaoHang { get; set; }

        public string GhiChu { get; set; }

        public string MaGiamGia { get; set; }

        [Display(Name = "Phí vận chuyển")]
        public decimal PhiVanChuyen { get; set; } = 0;
    }
}
