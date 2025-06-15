using Try.Domain;
using Try.DTO;

namespace Try.Repository;


public interface IOrderRepository : IRepository<Order>
{
    Task<List<Order>> GetOrdersByUserId(int userId);
    Task<TopProductDto> GetTopProductTodayAsync();

    Task<List<OrderDto>> GetAllOrdersAsync();
    
    List<(int, int, int)> GetAssociatedProducts();
    
    Task<TopProductTodayDto?> GetTopProductByDateAsync(DateTime date);

    Task<List<DateTime>> GetAllOrderDatesAsync();
    Task<List<TopProductTodayDto?>> GetProductsSoldByDateAsync(DateTime date);
    
    Task<List<TopProductTodayDto>> GetTop5ProductsAllTimeAsync();



}