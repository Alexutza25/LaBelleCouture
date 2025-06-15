using Try.Domain;

namespace Try.Repository.OrderDetailsRepository;

public interface IOrderDetailsRepository : IRepository<OrderDetails>
{
    Task<IEnumerable<OrderDetails>> GetOrderDetailsByUserId(int userId);

}