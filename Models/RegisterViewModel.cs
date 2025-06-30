namespace DATN.Models
{
    using System.ComponentModel.DataAnnotations;

    public class RegisterViewModel
    {
        [Required]
        public string? FirstName { get; set; }

        [Required]
        public string? FullName { get; set; }

        [Required, EmailAddress]
        public string? Email { get; set; }

        [Required, Phone]
        public string? PhoneNumber { get; set; }
        [Required, MaxLength(250)]
        public string? Adddress { get; set; }
        [Required, MinLength(6)]
        public string? Password { get; set; }

        [Required]
        [Compare("Password", ErrorMessage = "Mật khẩu xác nhận không khớp.")]
        public string? ConfirmPassword { get; set; }

        [Required]
        public DateTime BirthDate { get; set; }

        [Required]
        public string? Gender { get; set; }
    }

}
