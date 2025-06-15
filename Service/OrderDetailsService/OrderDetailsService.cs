using Try.Domain;
using Try.Repository.OrderDetailsRepository;

namespace Try.Service.OrderDetailsService;

public class OrderDetailsService : IOrderDetailsService
{
    private readonly IOrderDetailsRepository _orderdetailsRepository;

    public OrderDetailsService(IOrderDetailsRepository orderdetailsRepository)
    {
        _orderdetailsRepository = orderdetailsRepository;
    }

    public async Task<IEnumerable<OrderDetails>> GetAllOrderDetailss()
    {
        return await _orderdetailsRepository.GetAll();
    }

    public async Task<OrderDetails?> GetOrderDetailsById(int id)
    {
        return await _orderdetailsRepository.GetById(id);
    }

    public async Task<OrderDetails> CreateOrderDetails(OrderDetails orderdetails)
    {
        await _orderdetailsRepository.Add(orderdetails);
        return orderdetails;
    }

    public async Task<OrderDetails> UpdateOrderDetails(OrderDetails orderdetails)
    {
        await _orderdetailsRepository.Update(orderdetails);
        return orderdetails;
    }

    public async Task<bool> DeleteOrderDetails(int id)
    {
        var existing = await _orderdetailsRepository.GetById(id);
        if (existing == null) return false;
        await _orderdetailsRepository.Delete(id);
        return true;
    }
    public async Task<IEnumerable<OrderDetails>> GetOrderDetailsByUserId(int userId)
    {
        return await _orderdetailsRepository.GetOrderDetailsByUserId(userId);
    }

}