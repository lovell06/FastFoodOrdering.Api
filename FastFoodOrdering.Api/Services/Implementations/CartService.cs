using FastFoodOrdering.Api.Data;
using FastFoodOrdering.Api.DTOs.Cart;
using FastFoodOrdering.Api.Models;
using FastFoodOrdering.Api.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace FastFoodOrdering.Api.Services.Implementations
{
    public class CartService : ICartService
    {
        public readonly ApplicationDbContext _dbContext;   

        public CartService(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<bool> AddItemToCartAsync(int userId, AddToCartDto addToCartDto)
        {
            var product = await _dbContext.Products.FindAsync(addToCartDto.ProductId);
            
            if(product == null || !product.IsAvailable)
            {
                throw new InvalidOperationException("Sản phẩm không tồn tại hoặc đã hết hàng.");
            }

            var cart = await _dbContext.Carts.Include(c => c.Items).FirstOrDefaultAsync(c => c.UserId == userId);

            if(cart == null)
            {
                cart = new Cart { UserId = userId };
                _dbContext.Carts.Add(cart);
            }

            var cartItem = cart.Items.FirstOrDefault(i => i.ProductId == addToCartDto.ProductId);

            if(cartItem != null)
            {
                cartItem.Quantity += addToCartDto.Quantity;
            }
            else
            {
                cart.Items.Add(new CartItem
                {
                    ProductId = addToCartDto.ProductId,
                    Quantity = addToCartDto.Quantity
                });
            }

            await _dbContext.SaveChangesAsync();
            return true;

        }

        public async Task<Cart?> GetCartByUserIdAsync(int userId)
        {
            return await _dbContext.Carts
                .Include(c => c.Items)
                .ThenInclude(i => i.Product) 
                .AsNoTracking() 
                .FirstOrDefaultAsync(c => c.UserId == userId);
        }
        public async Task ClearCartAsync(int userId)
        {
            var cart = await _dbContext.Carts
                .Where(c => c.UserId == userId)
                .FirstOrDefaultAsync();

            if (cart != null)
            {
                var items = _dbContext.CartItems.Where(x => x.CartId == cart.Id);
                _dbContext.CartItems.RemoveRange(items);

                await _dbContext.SaveChangesAsync();
            }
        }
    }
}
