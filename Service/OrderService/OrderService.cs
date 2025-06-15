using Try.Domain;
using Try.DTO;
using Try.Repository;
using Try.Repository.UserRepository;

namespace Try.Service;

public class OrderService : IOrderService
{
    private readonly IOrderRepository _orderRepository;
    private readonly IUserRepository _userRepository;
    private readonly EmailService _emailService;

    public OrderService(IOrderRepository orderRepository, IUserRepository userRepository, EmailService emailService)
    {
        _orderRepository = orderRepository;
        _userRepository = userRepository;
        _emailService = emailService;
    }


    public async Task<IEnumerable<Order>> GetAllOrders()
    {
        return await _orderRepository.GetAll();
    }

    public async Task<Order?> GetOrderById(int id)
    {
        return await _orderRepository.GetById(id);
    }

    public async Task<Order> CreateOrder(CreateOrderDto dto)
    {
        var order = new Order
        {
            UserId = dto.UserId,
            PaymentMethod = dto.PaymentMethod,
            Date = DateTime.Now,
            Status = "Pending",
            Total = dto.OrderDetails.Sum(d => d.Subtotal),
            OrderDetails = dto.OrderDetails.Select(d => new OrderDetails
            {
                VariantId = d.VariantId,
                Quantity = d.Quantity,
                Price = d.Price,
                Subtotal = d.Subtotal
            }).ToList()
        };

        await _orderRepository.Add(order);

        var user = await _userRepository.GetById(dto.UserId); 

        _emailService.Send(
            user.Email,
            "Order confirmed",
            $@"<h2>Thank you for your order!</h2>
             <p>Your order has been received and is currently being processed.</p>
            <p><strong>Order Total:</strong> {order.Total:0.00} $</p>
            <p>We will notify you once it's ready for delivery.</p>" );


        return order;
    }


    public async Task<Order?> UpdateOrder(Order order)
    {
        await _orderRepository.Update(order);
        return order;
    }

    public async Task<bool> DeleteOrder(int id)
    {
        var order = await _orderRepository.GetById(id);
        if (order == null)
        {
            return false;
        }

        await _orderRepository.Delete(id);
        return true;
    }
    
    public async Task<IEnumerable<Order>> GetOrdersByUserId(int userId)
    {
        return await _orderRepository.GetOrdersByUserId(userId);
    }

    public Task<TopProductDto> GetTopProductsTodayAsync()
    {
        return _orderRepository.GetTopProductTodayAsync();
    }
    
    public Task<List<OrderDto>> GetAllOrdersAsync()
    {
        return _orderRepository.GetAllOrdersAsync();
    }
    
    public List<(int, int, int)> GetAssociatedProducts()
    {
        return _orderRepository.GetAssociatedProducts();
    }

    public async Task<TopProductTodayDto?> GetTopProductByDateAsync(DateTime date)
    {
        return await _orderRepository.GetTopProductByDateAsync(date);
    }
    
    public async Task<List<DateTime>> GetAllOrderDatesAsync()
    {
        return await _orderRepository.GetAllOrderDatesAsync();
    }
    
    public async Task<List<TopProductTodayDto>> GetProductsSoldByDateAsync(DateTime date)
    {
        return await _orderRepository.GetProductsSoldByDateAsync(date);
    }

    public async Task<List<TopProductTodayDto>> GetTop5ProductsAllTimeAsync()
    {
        return await _orderRepository.GetTop5ProductsAllTimeAsync();
    }



}