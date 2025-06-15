using Azure.Core;
using Microsoft.EntityFrameworkCore;
using Try.Domain;
using Try.DTO;
using Try.Repository;

namespace Try.Repository;
public class OrderRepository : GenericRepository<Order>, IOrderRepository
{
    private readonly ApplicationDbContext _context;

    public OrderRepository(ApplicationDbContext context) : base(context)
    {
        _context = context;
    }

    public async Task<TopProductDto> GetTopProductTodayAsync()
    {
        var today = DateTime.Today;

        var topProduct = await _context.OrderDetails
            .Include(od => od.ProductVariant)
            .ThenInclude(pv => pv.Product)
            .Include(od => od.Order)
            .Where(od => od.Order.Date.Date == today) 
            .GroupBy(od => new {
                od.ProductVariant.Product.ProductId,
                od.ProductVariant.Product.Name
            })
            .Select(g => new TopProductDto
            {
                ProductId = g.Key.ProductId,
                Name = g.Key.Name,
                Quantity = g.Sum(x => x.Quantity)
            })
            .FirstOrDefaultAsync();

        return topProduct ?? new TopProductDto { Name = "No orders", Quantity = 0 };
    }
    
    public async Task<List<Order>> GetOrdersByUserId(int userId)
    {
        return await _context.Orders
            .Where(o => o.UserId == userId)
            .Include(o => o.OrderDetails)
            .ThenInclude(od => od.ProductVariant)
            .ThenInclude(pv => pv.Product)
            .ToListAsync();
    }
    
    public async Task<List<OrderDto>> GetAllOrdersAsync()
    {
        return await _context.Orders
            .Include(o => o.User)
            .Include(o => o.OrderDetails)
            .Select(o => new OrderDto
            {
                OrderId = o.OrderId,
                UserId = o.UserId,
                Date = o.Date,
                Total = o.OrderDetails.Sum(x => x.Subtotal),
                Status = o.Status,
                PaymentMethod = o.PaymentMethod,
            })
            .ToListAsync();
    }
    
    public List<(int, int, int)> GetAssociatedProducts()
    {
        var tranzactii = _context.OrderDetails
            .Include(od => od.ProductVariant)
            .ThenInclude(pv => pv.Product)
            .GroupBy(od => od.OrderId)
            .Select(g => g.Select(x => x.ProductVariant.ProductId).ToList())
            .ToList();
        // se extrag produsele din Order
        var perechi = new Dictionary<(int, int), int>();

        foreach (var tranzactie in tranzactii) // se ia fiecare tranzactie
        {
            var produse = tranzactie.Distinct().ToList(); // se iau produsele
            for (int i = 0; i < produse.Count; i++)
            {
                for (int j = i + 1; j < produse.Count; j++)
                {
                    var key = (Math.Min(produse[i], produse[j]), Math.Max(produse[i], produse[j]));
                    if (perechi.ContainsKey(key))
                        perechi[key]++;
                    else
                        perechi[key] = 1; 
                }
                //se fac perechile de produse
            }
        }

        return perechi
            .Where(p => p.Value >= 3)
            .OrderByDescending(p => p.Value)
            .Take(10)
            .Select(p => (p.Key.Item1, p.Key.Item2, p.Value))
            .ToList();
        // se returneaza perechile care au aparut mai mult de 3 ori
    }
    public async Task<TopProductTodayDto?> GetTopProductByDateAsync(DateTime date)
    {
        var ordersInDay = await _context.Orders
            .Include(o => o.OrderDetails)
            .ThenInclude(od => od.ProductVariant)
            .ThenInclude(pv => pv.Product)
            .Where(o => o.Date.Date == date.Date) 
            .ToListAsync();

        var productStats = ordersInDay
            .SelectMany(o => o.OrderDetails)
            .GroupBy(od => od.ProductVariant.Product)
            .Select(g => new {
                Product = g.Key,
                Quantity = g.Sum(x => x.Quantity)
            })
            .OrderByDescending(x => x.Quantity)
            .FirstOrDefault();

        if (productStats == null)
            return null;

        var p = productStats.Product;

        return new TopProductTodayDto
        {
            ProductId = p.ProductId,
            Name = p.Name,
            ImageURL = p.ImageURL,
            Price = p.Price,
            Quantity = productStats.Quantity
        };
    }

    
    public async Task<List<DateTime>> GetAllOrderDatesAsync()
    {
        return await _context.Orders
            .Where(o => o.OrderDetails.Any(od => od.Quantity > 0 && od.ProductVariant != null))
            .Select(o => o.Date.Date)
            .Distinct()
            .OrderByDescending(d => d)
            .ToListAsync();
    }

    public async Task<List<TopProductTodayDto?>> GetProductsSoldByDateAsync(DateTime date)
    {
        var orders = await _context.Orders
            .Where(o => o.Date.Date == date.Date) // ← AICI schimbarea importantă
            .Include(o => o.OrderDetails)
            .ThenInclude(od => od.ProductVariant)
            .ThenInclude(pv => pv.Product)
            .ToListAsync();

        var result = orders
            .SelectMany(o => o.OrderDetails)
            .GroupBy(od => od.ProductVariant.Product)
            .Select(g => new TopProductTodayDto
            {
                ProductId = g.Key.ProductId,
                Name = g.Key.Name,
                Price = g.Key.Price,
                Quantity = g.Sum(x => x.Quantity)
            })
            .OrderByDescending(p => p.Quantity)
            .ToList();

        return result;
    }

    public async Task<List<TopProductTodayDto>> GetTop5ProductsAllTimeAsync()
    {
        var result = await _context.OrderDetails
            .Include(od => od.ProductVariant)
            .ThenInclude(pv => pv.Product)
            .GroupBy(od => od.ProductVariant.Product)
            .Select(g => new TopProductTodayDto
            {
                ProductId = g.Key.ProductId,
                Name = g.Key.Name,
                ImageURL = g.Key.ImageURL,
                Price = g.Key.Price,
                Quantity = g.Sum(x => x.Quantity)
            })
            .OrderByDescending(p => p.Quantity)
            .Take(5)
            .ToListAsync();

        return result;
    }





}