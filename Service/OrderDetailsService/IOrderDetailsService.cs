using Try.Domain;

namespace Try.Service.OrderDetailsService;

public interface IOrderDetailsService
{
    Task<IEnumerable<OrderDetails>> GetAllOrderDetailss();
    Task<OrderDetails?> GetOrderDetailsById(int id);
    Task<OrderDetails> CreateOrderDetails(OrderDetails orderdetails);
    Task<OrderDetails> UpdateOrderDetails(OrderDetails orderdetails);
    Task<bool> DeleteOrderDetails(int id);
    Task<IEnumerable<OrderDetails>> GetOrderDetailsByUserId(int userId);

}