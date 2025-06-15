using Microsoft.EntityFrameworkCore;
using Try.Domain;

namespace Try.Repository.OrderDetailsRepository;

public class OrderDetailsRepository : GenericRepository<OrderDetails>, IOrderDetailsRepository
{
    private readonly ApplicationDbContext _context;

    public OrderDetailsRepository(ApplicationDbContext context) : base(context)
    {
        _context = context;
    }

    public async Task<IEnumerable<OrderDetails>> GetOrderDetailsByUserId(int userId)
    {
        return await _context.OrderDetails
            .Where(od => od.Order.UserId == userId) // relatie prin Order
            .Include(od => od.Order) // include Order daca nu se face automat
            .ToListAsync();
    }
}