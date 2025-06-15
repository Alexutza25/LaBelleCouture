using Try.Domain;
using Try.DTO;

namespace Try.Service.CartService;

public interface ICartService
{
    Task<IEnumerable<Cart>> GetAllCarts();
    Task<Cart?> GetCartById(int id);
    Task<Cart> CreateCart(Cart cart);
    Task<Cart> UpdateCart(Cart cart);
    Task<bool> DeleteCart(int id);
    Task<IEnumerable<Cart>> GetCartByUserId(int userId);
    Task<bool> AddToCart(CartDto dto);
    Task<bool> UpdateQuantity(int cartId, int quantity);

    Task ClearCartByUserId(int userId);



}