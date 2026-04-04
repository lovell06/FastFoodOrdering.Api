using System.Text.Json.Serialization;


namespace FastFoodOrdering.Api.Models
{
    public class CartItem
    {
        public int Id { get; set; }
        public int ProductId { get; set; }
        public int Quantity { get; set; }
        public int CartId { get; set; }

        public virtual Product Product { get; set; }

        [JsonIgnore]
        public virtual Cart Cart { get; set; }
    }
}
