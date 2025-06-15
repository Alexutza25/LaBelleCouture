using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Try.Domain;
using Try.Service.OrderDetailsService;

namespace Try.Controller;

[ApiController]
[Route("api/[controller]")]
public class OrderDetailsController : ControllerBase
{
    private readonly IOrderDetailsService _orderdetailsService;

    public OrderDetailsController(IOrderDetailsService orderdetailsService)
    {
        _orderdetailsService = orderdetailsService;
    }

    [HttpGet]
    [EndpointSummary("afiseaza toate detaliile comenzilor")]
    public async Task<IActionResult> GetAll()
    {
        var result = await _orderdetailsService.GetAllOrderDetailss();
        return Ok(result);
    }

    [HttpGet("{id}")]
    [EndpointSummary("afiseaza detalii de comanda după ID")]
    public async Task<IActionResult> GetById(int id)
    {
        var result = await _orderdetailsService.GetOrderDetailsById(id);
        if (result == null) return NotFound();
        return Ok(result);
    }

    [HttpPost]
    [EndpointSummary("adauga detalii pentru o comanda")]
    public async Task<IActionResult> Create([FromBody] OrderDetails item)
    {
        var created = await _orderdetailsService.CreateOrderDetails(item);
        return CreatedAtAction(nameof(GetById), new { id = created.OrderDetailsId }, created);
    }

    [HttpPut("{id}")]
    [EndpointSummary("actualizeaza detaliile unei comenzi")]
    public async Task<IActionResult> Update(int id, [FromBody] OrderDetails item)
    {
        if (item.OrderDetailsId != id) return BadRequest();
        var updated = await _orderdetailsService.UpdateOrderDetails(item);
        return Ok(updated);
    }

    [HttpDelete("{id}")]
    [EndpointSummary("sterge detalii de comanda după ID")]
    public async Task<IActionResult> Delete(int id)
    {
        var deleted = await _orderdetailsService.DeleteOrderDetails(id);
        if (!deleted) return NotFound();
        return NoContent();
    }
    
    [Authorize]
    [HttpGet("me")]
    [EndpointSummary("afiseaza toate detaliile comenzilor pentru utilizatorul logat")]
    public async Task<IActionResult> GetMyOrderDetails()
    {
        var userId = int.Parse(User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value!);
        var orderDetails = await _orderdetailsService.GetOrderDetailsByUserId(userId);
        return Ok(orderDetails);
    }

}