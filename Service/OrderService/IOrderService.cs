using Try.Domain;
using Try.DTO;

namespace Try.Service;

public interface IOrderService
{
    public Task<IEnumerable<Order?>> GetAllOrders();
    public Task<Order?> GetOrderById(int id);
    public Task<Order?> CreateOrder(CreateOrderDto order);
    public Task<Order?> UpdateOrder(Order order);
    public Task<bool> DeleteOrder(int id);
    
     public Task<IEnumerable<Order>> GetOrdersByUserId(int userId);
     
     public Task<TopProductDto> GetTopProductsTodayAsync();
     
     public Task<List<OrderDto>> GetAllOrdersAsync();

     List<(int, int, int)> GetAssociatedProducts();
     
     Task<TopProductTodayDto?> GetTopProductByDateAsync(DateTime date);
     
     Task<List<DateTime>> GetAllOrderDatesAsync();
     
     Task<List<TopProductTodayDto>> GetProductsSoldByDateAsync(DateTime date);

     Task<List<TopProductTodayDto>> GetTop5ProductsAllTimeAsync();



}