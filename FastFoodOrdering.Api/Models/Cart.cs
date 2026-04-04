using System.Text.Json.Serialization;

namespace FastFoodOrdering.Api.Models
{
    public class Cart
    {
        public int Id { get; set; }
        public int UserId { get; set; } // Liên kết với người dùng

        [JsonIgnore]
        public virtual User User { get; set; }

        // 1 Cart có nhiều CartItems
        public virtual ICollection<CartItem> Items { get; set; } = new List<CartItem>();
    }
}
