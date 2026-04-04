using System.ComponentModel.DataAnnotations;

namespace FastFoodOrdering.Api.DTOs.Cart
{
    public class AddToCartDto
    {
        [Required]
        public int ProductId { get; set; }

        [Required]
        [Range(1, 100, ErrorMessage = "Số lượng phải từ 1 trở lên.")]
        public int Quantity { get; set; }
    }
}
