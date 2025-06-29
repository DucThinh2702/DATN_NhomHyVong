using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace DATN.Models;

public partial class Promotion
{
    [Key]
    public int PromoCode { get; set; }

    [Required(ErrorMessage = "Tên khuyến mãi không được bỏ trống")]
    [StringLength(100, ErrorMessage = "Tên khuyến mãi không được dài quá 100 ký tự")]
    public string? PromoName { get; set; }

    [Required(ErrorMessage = "Loại khuyến mãi không được để trống")]
    [StringLength(50, ErrorMessage = "Loại khuyến mãi không được dài quá 50 ký tự")]
    public string? PromoType { get; set; }

    [Range(0, 1000000, ErrorMessage = "Giá trị giảm phải lớn hơn 0")]
    [DisplayFormat(DataFormatString = "{0:0}", ApplyFormatInEditMode = true)]
    public decimal? DiscountValue { get; set; }

    [Range(0, 1000000, ErrorMessage = "Đơn hàng tối thiểu phải lớn hơn 0")]
    [DisplayFormat(DataFormatString = "{0:0}", ApplyFormatInEditMode = true)]
    public decimal? MinOrderAmount { get; set; }

    [DataType(DataType.Date)]
    [Required(ErrorMessage = "Vui lòng chọn ngày bắt đầu")]
    public DateTime? StartDate { get; set; }

    [DataType(DataType.Date)]
    [Required(ErrorMessage = "Vui lòng chọn ngày kết thúc")]
    public DateTime? EndDate { get; set; }

    [Required(ErrorMessage = "Số lượng khuyến mãi là bắt buộc")]
    [Range(1, 100000, ErrorMessage = "Số lượng phải từ 1 đến 100000")]
    public int? Quantity { get; set; }

    [Range(0, 100000, ErrorMessage = "Số lượng đã sử dụng không được âm")]
    public int? UsedQuantity { get; set; }

    [StringLength(50)]
    public string? Status { get; set; }

    public virtual ICollection<Order> Orders { get; set; } = new List<Order>();
}
