using FastFoodOrdering.Api.Enums;
using System.ComponentModel.DataAnnotations;

namespace FastFoodOrdering.Api.DTOs.Order;

public class CreateOrderRequestDto
{
    [Required(ErrorMessage = "Vui lòng nhập họ và tên người nhận.")]
    public string FullName { get; set; } = string.Empty;

    [Required(ErrorMessage = "Vui lòng nhập số điện thoại.")]
    [Phone(ErrorMessage = "Số điện thoại không đúng định dạng.")]
    public string Phone { get; set; } = string.Empty;

    [Required(ErrorMessage = "Vui lòng nhập địa chỉ giao hàng.")]
    public string Address { get; set; } = string.Empty;

    public string? Note { get; set; }

    public PaymentMethod PaymentMethod { get; set; } = PaymentMethod.Cash;
}
