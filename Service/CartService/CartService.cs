using Try.Domain;
using Try.DTO;
using Try.Repository.CartRepository;

namespace Try.Service.CartService;

public class CartService : ICartService
{
    private readonly ICartRepository _cartRepository;

    public CartService(ICartRepository cartRepository)
    {
        _cartRepository = cartRepository;
    }

    public async Task<IEnumerable<Cart>> GetAllCarts()
    {
        return await _cartRepository.GetAll();
    }

    public async Task<Cart?> GetCartById(int id)
    {
        return await _cartRepository.GetById(id);
    }

    public async Task<Cart> CreateCart(Cart cart)
    {
        await _cartRepository.Add(cart);
        return cart;
    }

    public async Task<Cart> UpdateCart(Cart cart)
    {
        await _cartRepository.Update(cart);
        return cart;
    }

    public async Task<bool> DeleteCart(int id)
    {
        var existing = await _cartRepository.GetById(id);
        if (existing == null) return false;
        await _cartRepository.Delete(id);
        return true;
    }
    
    public async Task<IEnumerable<Cart>> GetCartByUserId(int userId)
    {
        return await _cartRepository.GetCartByUserId(userId);
    }
    
    public async Task<bool> AddToCart(CartDto dto)
    {
        var user = await _cartRepository.GetCartByUserId(dto.UserId);
        var variant = await _cartRepository.GetVariantById(dto.VariantId);

        if (user == null || variant == null)
            return false;

        var existing = await _cartRepository.GetCartByUserAndVariant(dto.UserId, dto.VariantId);


        if (existing != null)
        {
            existing.Quantity += dto.Quantity;
        }
        else
        {
            var cart = new Cart
            {
                UserId = dto.UserId,
                VariantId = dto.VariantId,
                Quantity = dto.Quantity
            };
            await _cartRepository.Add(cart);
        }

        return true;
    }
    public async Task<bool> UpdateQuantity(int cartId, int quantity)
    {
        var cart = await _cartRepository.GetById(cartId);
        if (cart == null) return false;

        cart.Quantity = quantity;
        await _cartRepository.Update(cart);
        return true;
    }

    public async Task ClearCartByUserId(int userId)
    {
        var cartItems = await _cartRepository.GetCartByUserId(userId); 
        foreach (var item in cartItems)
        {
            await _cartRepository.Delete(item.CartId);
        }
    }


}