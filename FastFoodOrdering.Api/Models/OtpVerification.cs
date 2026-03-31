using System.ComponentModel.DataAnnotations;

namespace FastFoodOrdering.Api.Models
{
    public class OtpVerification
    {
        public int Id { get; set; }

        [MaxLength(100)]
        public string Email { get; set; } = string.Empty;

        [MaxLength(6)]
        public string OtpCode { get; set; } = string.Empty;

        public DateTime ExpiryTime { get; set; } // Thời gian hết hạn

        public bool IsUsed { get; set; } // Đánh dấu OTP đã sử dụng chưa
    }
}
