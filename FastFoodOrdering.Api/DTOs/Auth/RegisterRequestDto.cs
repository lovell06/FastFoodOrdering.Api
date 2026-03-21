using System.ComponentModel.DataAnnotations;

namespace FastFoodOrdering.Api.DTOs.Auth;

public class RegisterRequestDto
{
    // 1. Họ và tên
    [Required(ErrorMessage = "Họ và tên không được bỏ trống.")]
    public string FullName { get; set; } = string.Empty;

    // 2. Email
    [Required(ErrorMessage = "Email không được bỏ trống.")]
    [EmailAddress(ErrorMessage = "Vui lòng nhập đúng định dạng email.")]
    public string Email { get; set; } = string.Empty;

    // 3. Số điện thoại 
    [Required(ErrorMessage = "Số điện thoại không được bỏ trống.")]
    [Phone(ErrorMessage = "Vui lòng nhập đúng định dạng số điện thoại.")]
    public string Phone { get; set; } = string.Empty;

    // 4. Mật khẩu
    [Required(ErrorMessage = "Mật khẩu không được bỏ trống.")]
    [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d).{8,}$",
        ErrorMessage = "Mật khẩu phải có ít nhất 8 ký tự, bao gồm ít nhất 1 chữ hoa, 1 chữ thường và 1 chữ số.")]
    public string Password { get; set; } = string.Empty;

    // 5. Xác nhận mật khẩu
    [Required(ErrorMessage = "Xác nhận mật khẩu không được bỏ trống.")]
    [Compare("Password", ErrorMessage = "Mật khẩu xác nhận không trùng khớp.")]
    public string ConfirmPassword { get; set; } = string.Empty;
}
