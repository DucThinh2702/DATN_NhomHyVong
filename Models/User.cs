using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace DATN.Models;

public partial class User
{
    public int UserId { get; set; }

    [Required(ErrorMessage = "Họ tên không được để trống")]
    [StringLength(100)]
    public string? FullName { get; set; }

    [Required(ErrorMessage = "Tên đăng nhập không được để trống")]
    [StringLength(50)]
    public string? Username { get; set; }

    [Required(ErrorMessage = "Email không được để trống")]
    [EmailAddress(ErrorMessage = "Email không hợp lệ")]
    public string? Email { get; set; }

    [Required(ErrorMessage = "Mật khẩu không được để trống")]
    [StringLength(100, MinimumLength = 6, ErrorMessage = "Mật khẩu phải từ 6 đến 100 ký tự")]
    public string? Password { get; set; }

    [Required(ErrorMessage = "Giới tính không được để trống")]
    public string? Gender { get; set; }

    [Required(ErrorMessage = "Ngày sinh không được để trống")]
    [Range(typeof(DateTime), "01/01/1900", "01/01/2010", ErrorMessage = "Ngày sinh phải từ 1900 đến 2010")]
    public DateOnly? BirthDate { get; set; }

    [Required(ErrorMessage = "Số điện thoại không được để trống")]
    [Phone(ErrorMessage = "Số điện thoại không hợp lệ")]
    public string? PhoneNumber { get; set; }

    [Required(ErrorMessage = "Địa chỉ không được để trống")]
    public string? Address { get; set; }

    [DataType(DataType.Date)]
    public DateTime CreatedDate { get; set; } = DateTime.Now;

    public bool? Status { get; set; } = true;

    [Required(ErrorMessage = "Vai trò không được để trống")]
    public int? RoleId { get; set; } = 1;
    public virtual Cart? Cart { get; set; }

    public virtual ICollection<News>? News { get; set; } = [];

    public virtual ICollection<Order>? Orders { get; set; } = [];

    public virtual Role? Role { get; set; }
}
