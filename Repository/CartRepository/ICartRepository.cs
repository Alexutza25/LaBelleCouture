using Try.Domain;

namespace Try.Repository.CartRepository;

public interface ICartRepository : IRepository<Cart>
{
    Task<IEnumerable<Cart>> GetCartByUserId(int userId);
    Task<User?> GetUserById(int id);
    Task<ProductVariant?> GetVariantById(int id);
    Task<Cart?> GetCartByUserAndVariant(int userId, int variantId);
    



}