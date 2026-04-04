using System.ComponentModel.DataAnnotations;

namespace FastFoodOrdering.Api.DTOs.Auth
{
    public class ResetPasswordDto
    {
        [Required(ErrorMessage = "Vui lòng nhập Email.")]
        [EmailAddress(ErrorMessage = "Email không đúng định dạng.")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "Vui lòng nhập mã OTP.")]
        [StringLength(6, MinimumLength = 6, ErrorMessage = "Mã OTP phải bao gồm 6 số.")]
        public string OtpCode { get; set; } = string.Empty;

        // AC2: Mật khẩu mới phải đạt quy tắc bảo mật
        [Required(ErrorMessage = "Vui lòng nhập mật khẩu mới.")]
        [MinLength(6, ErrorMessage = "Mật khẩu phải có ít nhất 6 ký tự.")]
        public string NewPassword { get; set; } = string.Empty;

        // AC2: Xác nhận mật khẩu phải khớp
        [Required(ErrorMessage = "Vui lòng xác nhận mật khẩu.")]
        [Compare("NewPassword", ErrorMessage = "Xác nhận mật khẩu không khớp.")]
        public string ConfirmPassword { get; set; } = string.Empty;
    }
}
