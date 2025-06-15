using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Try.Domain;
using Try.DTO;
using Try.Service;

namespace Try.Controller;

[ApiController]
[Route("api/[controller]")]
public class OrderController : ControllerBase
{
    private readonly IOrderService _orderService;


    public OrderController(IOrderService orderService)
    {
        _orderService = orderService;

    }
    
    [HttpGet]
    [EndpointSummary("afiseaza toate comenzile")]
    public async Task<IActionResult> GetAll()
    {
        var result = await _orderService.GetAllOrders();
        return Ok(result);
    }

    [HttpGet("{id}")]
    [EndpointSummary("afiseaza o comandă după ID")]
    public async Task<IActionResult> GetById(int id)
    {
        var result = await _orderService.GetOrderById(id);
        if (result == null) return NotFound();
        return Ok(result);
    }

    [HttpPost]
    public async Task<IActionResult> CreateOrder([FromBody] CreateOrderDto dto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        try
        {
            var createdOrder = await _orderService.CreateOrder(dto);
            return Ok(createdOrder);
        }
        catch (Exception ex)
        {
            Console.WriteLine("❌ Order creation failed: " + ex.Message);
            return BadRequest("Something went wrong while creating the order.");
        }
   
    }


    [HttpPut("{id}")]
    [EndpointSummary("actualizeaza o comanda existenta")]
    public async Task<IActionResult> Update(int id, [FromBody] Order item)
    {
        if (item.OrderId != id) return BadRequest();
        var updated = await _orderService.UpdateOrder(item);
        return Ok(updated);
    }

    [HttpDelete("{id}")]
    [EndpointSummary("sterge o comanda după id")]
    public async Task<IActionResult> Delete(int id)
    {
        var deleted = await _orderService.DeleteOrder(id);
        if (!deleted) return NotFound();
        return NoContent();
    }
    
    [Authorize]
    [HttpGet("me")]
    [EndpointSummary("afiseaza comenzile utilizatorului logat")]
    public async Task<IActionResult> GetMyOrders()
    {
        var userId = int.Parse(User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value!);
        var orders = await _orderService.GetOrdersByUserId(userId);
        return Ok(orders);
    }
    
    [HttpGet("user/{userId}")]
    public async Task<IActionResult> GetOrdersByUserId(int userId)
    {
        try
        {
            var orders = await _orderService.GetOrdersByUserId(userId);
            if (orders == null || !orders.Any())
                return Ok(new List<Order>());

            return Ok(orders);
        }
        catch (Exception ex)
        {
            Console.WriteLine("EROARE la GetOrdersByUserId: " + ex.Message);
            return StatusCode(500, "Error loading orders");
        }
    }
    
    [HttpGet("top-product-today")]
    public async Task<IActionResult> GetTopProductToday()
    {
        var result = await _orderService.GetTopProductsTodayAsync();
        return Ok(result);
    }

    [HttpGet("admin-all")]
    public async Task<IActionResult> GetAllOrdersForAdmin()
    {
        var orders = await _orderService.GetAllOrdersAsync();
        return Ok(orders);
    }
    
    [HttpGet("variant-associations")]
    public IActionResult GetAssociatedVariants()
    {
        var asociate = _orderService.GetAssociatedProducts();

        var result = asociate.Select(p => new ProductAssociationDto
        {
            Product1Id = p.Item1,
            Product2Id = p.Item2,
            Count = p.Item3
        }).ToList();

        return Ok(result);
    }

    [HttpGet("top-product-date")]
    public async Task<IActionResult> GetTopProductByDate([FromQuery] DateTime date)
    {
        var product = await _orderService.GetTopProductByDateAsync(date);
        if (product == null)
            return NotFound("No product found for selected date");

        return Ok(product);
    }

    [HttpGet("dates")]
    public async Task<IActionResult> GetAllOrderDates()
    {
        var dates = await _orderService.GetAllOrderDatesAsync();
        return Ok(dates);
    }

    [HttpGet("products-by-date")]
    public async Task<IActionResult> GetProductsSoldByDate([FromQuery] DateTime date)
    {
        var products = await _orderService.GetProductsSoldByDateAsync(date);
        return Ok(products);
    }
    
    [HttpGet("top5-alltime")]
    public async Task<IActionResult> GetTop5AllTime()
    {
        var top = await _orderService.GetTop5ProductsAllTimeAsync();
        return Ok(top);
    }


}