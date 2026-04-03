using System.ComponentModel.DataAnnotations;

namespace FastFoodOrdering.Api.DTOs.Auth
{
    public class SendOtpRequestDto
    {
        [Required(ErrorMessage = "Email không được bỏ trống.")]
        [EmailAddress(ErrorMessage = "Vui lòng nhập đúng định dạng email.")]
        public string Email { get; set; } = string.Empty;
    }
}
